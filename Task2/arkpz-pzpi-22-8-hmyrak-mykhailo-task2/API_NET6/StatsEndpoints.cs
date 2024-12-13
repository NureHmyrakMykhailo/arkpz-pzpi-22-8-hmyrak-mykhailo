using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using API_NET6.Models;
using Microsoft.EntityFrameworkCore;

public static class StatsEndpoints
{
    public static void MapStatsEndpoints(this IEndpointRouteBuilder endpoints, lib4Context db)
    {
        endpoints.MapGet("/stat", () =>
        {
            var bookTitlesCount = db.Books.Count();
            var bookItemsCount = db.Items.Count();
            var itemsOnLoanCount = db.Items.Count(i => i.ReaderId.HasValue);
            var availableItemsCount = db.Items.Count(i => i.Available);
            var specialStorageCount = bookItemsCount - (itemsOnLoanCount + availableItemsCount);
            var readersCount = db.Readers.Count();
            var activeReadersCount = db.Hists.Select(h => h.ReaderId).Distinct().Count();
            var popularBookTitlesCount = db.Books.Count(b => b.Items.Any(i => db.Hists.Any(h => h.ItemId == i.ItemId)));

            return Results.Ok(new
            {
                BookTitlesCount = bookTitlesCount,
                BookItemsCount = bookItemsCount,
                ItemsOnLoanCount = itemsOnLoanCount,
                AvailableItemsCount = availableItemsCount,
                SpecialStorageCount = specialStorageCount,
                ReadersCount = readersCount,
                ActiveReadersCount = activeReadersCount,
                PopularBookTitlesCount = popularBookTitlesCount
            });
        }).RequireAuthorization();
    }
}
