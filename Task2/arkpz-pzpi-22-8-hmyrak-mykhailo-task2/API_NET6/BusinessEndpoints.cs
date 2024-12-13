using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using API_NET6.Models;
using Microsoft.EntityFrameworkCore;

public static class BusinessEndpoints
{
    public static void MapBusinessEndpoints(this IEndpointRouteBuilder endpoints, lib4Context db)
    {
        endpoints.MapPost("/business/additems", (AddItemsRequest request) =>
        {
            var book = db.Books.Find(request.BookId);
            if (book == null)
            {
                return Results.NotFound();
            }

            var items = new List<Item>();
            for (int i = 0; i < request.Count; i++)
            {
                var item = new Item
                {
                    BookId = request.BookId,
                    Description = request.Description,
                    Available = request.Available
                };
                db.Items.Add(item);
                items.Add(item);
            }

            db.SaveChanges();

            foreach (var item in items)
            {
                var hist = new Hist
                {
                    StatusId = 1,
                    ItemId = item.ItemId
                };
                db.Hists.Add(hist);
            }

            db.SaveChanges();

            var itemIds = items.Select(i => i.ItemId).ToArray();
            return Results.Ok(itemIds);
        }).RequireAuthorization();



        endpoints.MapPost("/business/giveout", (GiveBookRequest request) =>
        {
            var reader = db.Readers.Find(request.ReaderId);
            if (reader == null)
            {
                return Results.NotFound();
            }

            Item? item;
            if (request.ItemId.HasValue)
            {
                item = db.Items.FirstOrDefault(i => i.ItemId == request.ItemId && i.Available == true);
            }
            else
            {
                item = db.Items.FirstOrDefault(i => i.BookId == request.BookId && i.Available == true);
            }

            if (item == null)
            {
                return Results.NotFound("No available item found.");
            }

            item.Available = false;
            item.ReaderId = request.ReaderId;

            var hist = new Hist
            {
                StatusId = 2,
                ItemId = item.ItemId,
                ReaderId = request.ReaderId,
                Comment = request.Comment

            };
            db.Hists.Add(hist);

            db.SaveChanges();
            return Results.Ok(item.ItemId);
        }).RequireAuthorization();



        endpoints.MapPost("/business/return", (ReturnBookRequest request) =>
        {
            if (request.ReaderId.HasValue && !request.ItemId.HasValue)
            {
                var items = db.Items.Where(i => i.ReaderId == request.ReaderId).ToList();
                if (!items.Any())
                {
                    return Results.NotFound("No items found for the specified reader.");
                }

                foreach (var item in items)
                {
                    item.Available = true;
                    item.ReaderId = null;

                    var hist = new Hist
                    {
                        StatusId = 3,
                        ItemId = item.ItemId,
                        ReaderId = request.ReaderId,
                        Comment = request.Comment
                    };
                    db.Hists.Add(hist);
                }

                db.SaveChanges();
                var itemIds = items.Select(i => i.ItemId).ToArray();
                return Results.Ok(itemIds);
            }
            else
            {
                var item = db.Items.Find(request.ItemId);
                if (item == null)
                {
                    return Results.NotFound();
                }

                if (item.ReaderId == null)
                {
                    return Results.Ok("No action taken as the item has no associated reader.");
                }

                var currentReaderId = item.ReaderId;

                item.Available = true;
                item.ReaderId = null;

                var hist = new Hist
                {
                    StatusId = 3,
                    ItemId = item.ItemId,
                    ReaderId = request.ReaderId ?? currentReaderId,
                    Comment = request.Comment
                };
                db.Hists.Add(hist);

                db.SaveChanges();
                return Results.Ok(item.ItemId);
            }
        }).RequireAuthorization();
    }
}

public class AddItemsRequest
{
    public int BookId { get; set; }
    public string? Description { get; set; }
    public bool Available { get; set; } = true;
    public int Count { get; set; } = 1;
}

public class GiveBookRequest
{
    public int? BookId { get; set; }
    public int? ItemId { get; set; }
    public int ReaderId { get; set; }
    public string? Comment { get; set; }
}


public class ReturnBookRequest
{
    public int? ItemId { get; set; }
    public int? ReaderId { get; set; }
    public string? Comment { get; set; }
}
