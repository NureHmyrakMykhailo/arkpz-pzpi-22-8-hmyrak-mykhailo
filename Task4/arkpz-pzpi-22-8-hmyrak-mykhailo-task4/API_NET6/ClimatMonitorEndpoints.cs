using API_NET6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

public static class ClimatMonitorEndpoints
{
    public static void MapClimatMonitorEndpoints(this IEndpointRouteBuilder endpoints, libraryContext db)
    {

        endpoints.MapGet("/climatmonitors", [Authorize] (
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate) =>
        {
            var param = db.Params.FirstOrDefault();
            if (param == null)
            {
                return Results.NotFound("No parameters found.");
            }

            var tempMax = param.TempMax;
            var tempMin = param.TempMin;
            var wetMax = param.WetMax;
            var wetMin = param.WetMin;

            var query = db.ClimatMonitors.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(cm => cm.Time >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(cm => cm.Time <= endDate.Value);
            }

            var result = query.OrderBy(cm => cm.Time).ToList();

            double timeTemperatureAbove = 0;
            double timeTemperatureBelow = 0;
            double timeWetAbove = 0;
            double timeWetBelow = 0;

            var resultWithFlags = result.Select(cm => new
            {
                cm.Id,
                cm.Time,
                cm.Temperature,
                cm.Wet,
                cm.Pressure,
                IsTemperatureOutOfBounds = (tempMax.HasValue && cm.Temperature > tempMax.Value) || (tempMin.HasValue && cm.Temperature < tempMin.Value),
                IsWetOutOfBounds = (wetMax.HasValue && cm.Wet > wetMax.Value) || (wetMin.HasValue && cm.Wet < wetMin.Value)
            }).ToList();

            for (int i = 1; i < result.Count; i++)
            {
                var prev = result[i - 1];
                var current = result[i];

                if (tempMax.HasValue && prev.Temperature > tempMax.Value && current.Temperature > tempMax.Value)
                {
                    timeTemperatureAbove += (current.Time - prev.Time).TotalSeconds;
                }

                if (tempMin.HasValue && prev.Temperature < tempMin.Value && current.Temperature < tempMin.Value)
                {
                    timeTemperatureBelow += (current.Time - prev.Time).TotalSeconds;
                }

                if (wetMax.HasValue && prev.Wet > wetMax.Value && current.Wet > wetMax.Value)
                {
                    timeWetAbove += (current.Time - prev.Time).TotalSeconds;
                }

                if (wetMin.HasValue && prev.Wet < wetMin.Value && current.Wet < wetMin.Value)
                {
                    timeWetBelow += (current.Time - prev.Time).TotalSeconds;
                }
            }

            return Results.Ok(new { result = resultWithFlags, timeTemperatureAbove, timeTemperatureBelow, timeWetAbove, timeWetBelow });
        });



        endpoints.MapPost("/climatmonitors", [Authorize(Roles = "Sensor")] async (ClimatMonitorRequest request) =>
        {
            var climatMonitor = new ClimatMonitor
            {
                Temperature = request.Temperature,
                Wet = request.Wet,
                Pressure = request.Pressure,
            };

            db.ClimatMonitors.Add(climatMonitor);
            await db.SaveChangesAsync();
            return Results.Created($"/climatmonitors/{climatMonitor.Id}", climatMonitor);
        }).RequireAuthorization();
    }
}

public class ClimatMonitorRequest
{
    public double? Temperature { get; set; }
    public double? Wet { get; set; }
    public double? Pressure { get; set; }
}

