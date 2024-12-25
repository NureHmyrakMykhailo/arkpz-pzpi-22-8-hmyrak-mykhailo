using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using API_NET6.Models;
using Microsoft.EntityFrameworkCore;

public static class ItemsEndpoints
{
    public static void MapItemsEndpoints(this IEndpointRouteBuilder endpoints, libraryContext db)
    {
        endpoints.MapGet("/items", () => db.Items
            .Select(i => new
            {
                i.ItemId,
                i.BookId,
                i.ReaderId,
                i.Available,
                i.Description
            }).ToList())
            .RequireAuthorization();

        endpoints.MapGet("/items/{id}", (int id) =>
        {
            var item = db.Items
                .Include(i => i.Book)
                .Include(i => i.Reader)
                .Include(i => i.Hists)
                .Select(i => new
                {
                    i.ItemId,
                    i.BookId,
                    BookTitle = i.Book.Title,
                    i.ReaderId,
                    ReaderName = i.Reader.Name,
                    i.Available,
                    i.Description,
                    Hists = i.Hists.Select(h => new
                    {
                        h.HistId,
                        h.ReaderId,
                        h.StatusId,
                        h.Time
                    })
                })
                .FirstOrDefault(i => i.ItemId == id);

            if (item == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(item);
        }).RequireAuthorization();

        endpoints.MapPost("/items", (Item item) =>
        {
            db.Items.Add(item);
            db.SaveChanges();
            return item;
        }).RequireAuthorization();

        endpoints.MapPut("/items/{id}", (int id, Item updatedItem) =>
        {
            var existingItem = db.Items.Find(id);
            if (existingItem == null)
            {
                return Results.NotFound();
            }

            //existingItem.BookId = updatedItem.BookId;
            if (updatedItem.ReaderId != null)  existingItem.ReaderId = updatedItem.ReaderId;
            if (updatedItem.Available != null) existingItem.Available = updatedItem.Available;
            if (updatedItem.Description != null) existingItem.Description = updatedItem.Description;

            db.SaveChanges();
            return Results.Ok(existingItem);
        }).RequireAuthorization();

        endpoints.MapDelete("/items/{id}", (int id) =>
        {
            var existingItem = db.Items.Find(id);
            if (existingItem == null)
            {
                return Results.NotFound();
            }

            db.Items.Remove(existingItem);
            db.SaveChanges();
            return Results.NoContent();
        }).RequireAuthorization();
    }
}
