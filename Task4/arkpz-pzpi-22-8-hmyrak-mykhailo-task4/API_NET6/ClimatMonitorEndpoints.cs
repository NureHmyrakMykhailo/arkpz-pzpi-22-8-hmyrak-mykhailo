using API_NET6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

public static class ClimatMonitorEndpoints
{
    // Extension method to configure route mappings for climat monitor endpoints
    public static void MapClimatMonitorEndpoints(this IEndpointRouteBuilder endpoints, libraryContext db)
    {
        // Endpoint to get climat monitor data
        endpoints.MapGet("/climatmonitors", [Authorize] ( // Requires authorization
            [FromQuery] DateTime? startDate, // Optional parameter to filter data from a specific start date
            [FromQuery] DateTime? endDate) => // Optional parameter to filter data up to a specific end date
        {
            // Retrieve parameters from the database
            var param = db.Params.FirstOrDefault();
            if (param == null)
            {
                return Results.NotFound("No parameters found."); // Return error if no parameters exist
            }

            // Extract threshold values for temperature and wetness
            var tempMax = param.TempMax;
            var tempMin = param.TempMin;
            var wetMax = param.WetMax;
            var wetMin = param.WetMin;

            // Query to fetch climat monitor records
            var query = db.ClimatMonitors.AsQueryable();

            if (startDate.HasValue) // Filter records by start date
            {
                query = query.Where(cm => cm.Time >= startDate.Value);
            }

            if (endDate.HasValue) // Filter records by end date
            {
                query = query.Where(cm => cm.Time <= endDate.Value);
            }

            // Sort records by time
            var result = query.OrderBy(cm => cm.Time).ToList();

            // Variables to store durations of out-of-bound parameters
            double timeTemperatureAbove = 0;
            double timeTemperatureBelow = 0;
            double timeWetAbove = 0;
            double timeWetBelow = 0;

            // Add flags to results indicating out-of-bound parameter values
            var resultWithFlags = result.Select(cm => new
            {
                cm.Id,
                cm.Time,
                cm.Temperature,
                cm.Wet,
                cm.Pressure,
                IsTemperatureOutOfBounds = (tempMax.HasValue && cm.Temperature > tempMax.Value) ||
                                            (tempMin.HasValue && cm.Temperature < tempMin.Value),
                IsWetOutOfBounds = (wetMax.HasValue && cm.Wet > wetMax.Value) ||
                                   (wetMin.HasValue && cm.Wet < wetMin.Value)
            }).ToList();

            // Calculate the duration for which parameters were out of bounds
            for (int i = 1; i < result.Count; i++)
            {
                var prev = result[i - 1]; // Previous record
                var current = result[i]; // Current record

                // Calculate duration where temperature exceeds the maximum threshold
                if (tempMax.HasValue && prev.Temperature > tempMax.Value && current.Temperature > tempMax.Value)
                {
                    timeTemperatureAbove += (current.Time - prev.Time).TotalSeconds;
                }

                // Calculate duration where temperature is below the minimum threshold
                if (tempMin.HasValue && prev.Temperature < tempMin.Value && current.Temperature < tempMin.Value)
                {
                    timeTemperatureBelow += (current.Time - prev.Time).TotalSeconds;
                }

                // Calculate duration where wetness exceeds the maximum threshold
                if (wetMax.HasValue && prev.Wet > wetMax.Value && current.Wet > wetMax.Value)
                {
                    timeWetAbove += (current.Time - prev.Time).TotalSeconds;
                }

                // Calculate duration where wetness is below the minimum threshold
                if (wetMin.HasValue && prev.Wet < wetMin.Value && current.Wet < wetMin.Value)
                {
                    timeWetBelow += (current.Time - prev.Time).TotalSeconds;
                }
            }

            // Return results with calculations
            return Results.Ok(new
            {
                result = resultWithFlags,
                timeTemperatureAbove,
                timeTemperatureBelow,
                timeWetAbove,
                timeWetBelow
            });
        });

        // Endpoint to create a new climat monitor record
        endpoints.MapPost("/climatmonitors", [Authorize(Roles = "Sensor")] async (ClimatMonitorRequest request) => // Requires "Sensor" role authorization
        {
            // Create a new ClimatMonitor object
            var climatMonitor = new ClimatMonitor
            {
                Temperature = request.Temperature,
                Wet = request.Wet,
                Pressure = request.Pressure,
            };

            // Add the new record to the database
            db.ClimatMonitors.Add(climatMonitor);
            await db.SaveChangesAsync(); // Save changes to the database
            return Results.Created($"/climatmonitors/{climatMonitor.Id}", climatMonitor); // Return the created record
        }).RequireAuthorization();
    }
}

// Class to define the structure of a climat monitor creation request
public class ClimatMonitorRequest
{
    public double? Temperature { get; set; } // Temperature parameter
    public double? Wet { get; set; }        // Wetness parameter
    public double? Pressure { get; set; }   // Pressure parameter
}


