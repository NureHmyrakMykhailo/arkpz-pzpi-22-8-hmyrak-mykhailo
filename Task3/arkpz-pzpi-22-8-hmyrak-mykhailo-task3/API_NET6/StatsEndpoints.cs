using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using API_NET6.Models;
using Microsoft.EntityFrameworkCore;
public static class StatsEndpoints
{
    public static void MapStatsEndpoints(this IEndpointRouteBuilder endpoints, lib4Context db)
    {
        // Endpoint to get library statistics
        endpoints.MapGet("/stat", () =>
        {
            // Get the number of book titles in the library
            var bookTitlesCount = db.Books.Count();

            // Get the total number of book items
            var bookItemsCount = db.Items.Count();

            // Count of items currently on loan
            var itemsOnLoanCount = db.Items.Count(i => i.ReaderId.HasValue);

            // Count of available items
            var availableItemsCount = db.Items.Count(i => i.Available);

            // Count of items in special storage (not available and not on loan)
            var specialStorageCount = bookItemsCount - (itemsOnLoanCount + availableItemsCount);

            // Number of readers in the database
            var readersCount = db.Readers.Count();

            // Number of active readers (those with records in the usage history)
            var activeReadersCount = db.Hists.Select(h => h.ReaderId).Distinct().Count();

            // Count of popular book titles (those with records in the usage history)
            var popularBookTitlesCount = db.Books.Count(b => b.Items.Any(i => db.Hists.Any(h => h.ItemId == i.ItemId)));

            // Current time for reading time calculation
            var currentTime = DateTime.UtcNow;

            // Fetching usage history data for reading time calculation
            var rawData = db.Hists
                .Where(h => h.StatusId == 2 || h.StatusId == 3) // Filter by statuses "issued" and "returned"
                .ToList(); // Execute query immediately

            // Calculating reading time for each item
            var readingTimes = rawData
                .GroupBy(h => h.ItemId) // Group records by ItemId
                .Select(g => new
                {
                    ItemId = g.Key,
                    ReadingTime = g
                        .Where(h => h.StatusId == 2) // Start of reading (issued)
                        .Select(h => h.Time)
                        .DefaultIfEmpty(currentTime) // If no date, use current time
                        .Zip(
                            g.Where(h => h.StatusId == 3) // End of reading (returned)
                             .Select(h => h.Time)
                             .DefaultIfEmpty(currentTime),
                            (start, end) => (end - start).TotalMinutes // Calculate time in minutes
                        )
                })
                .SelectMany(x => x.ReadingTime) // Flatten the lists of reading times
                .ToList();

            // Average reading time
            var averageReadingTime = readingTimes.Any() ? readingTimes.Average() : 0;

            // Maximum reading time
            var maxReadingTime = readingTimes.Any() ? readingTimes.Max() : 0;

            // Return statistics in JSON format
            return Results.Ok(new
            {
                BookTitlesCount = bookTitlesCount, // Number of book titles
                BookItemsCount = bookItemsCount, // Total number of items
                ItemsOnLoanCount = itemsOnLoanCount, // Number of items on loan
                AvailableItemsCount = availableItemsCount, // Number of available items
                SpecialStorageCount = specialStorageCount, // Number of items in special storage
                ReadersCount = readersCount, // Number of readers
                ActiveReadersCount = activeReadersCount, // Number of active readers
                PopularBookTitlesCount = popularBookTitlesCount, // Number of popular book titles
                AverageReadingTime = averageReadingTime, // Average reading time
                MaxReadingTime = maxReadingTime // Maximum reading time
            });
        }).RequireAuthorization(); // Authorization required
    }
}
