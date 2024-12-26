using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using API_NET6.Models;
using Microsoft.EntityFrameworkCore;

public static class BusinessEndpoints
{
    public static void MapBusinessEndpoints(this IEndpointRouteBuilder endpoints, libraryContext db)
    {
        // Endpoint for adding new book items to the library catalog
        endpoints.MapPost("/business/additems", (AddItemsRequest request) =>
        {
            // Find the book by its BookId
            var book = db.Books.Find(request.BookId);
            if (book == null)
            {
                return Results.NotFound(); // Return 404 if the book is not found
            }

            var items = new List<Item>();

            // Create the required number of book items
            for (int i = 0; i < request.Count; i++)
            {
                var item = new Item
                {
                    BookId = request.BookId,
                    Description = request.Description,
                    Available = request.Available
                };
                db.Items.Add(item); // Add the item to the database
                items.Add(item); // Add the item to the local list
            }

            db.SaveChanges(); // Save the items to the database

            // For each item, add a record to the history with status "available" (StatusId = 1)
            foreach (var item in items)
            {
                var hist = new Hist
                {
                    StatusId = 1,
                    ItemId = item.ItemId
                };
                db.Hists.Add(hist); // Add the record to the history
            }

            db.SaveChanges(); // Save the history records to the database

            // Return a list of IDs of the created items
            var itemIds = items.Select(i => i.ItemId).ToArray();
            return Results.Ok(itemIds);
        }).RequireAuthorization();

        // Endpoint for issuing a book to a reader
        endpoints.MapPost("/business/giveout", (GiveBookRequest request) =>
        {
            // Find the reader by ReaderId
            var reader = db.Readers.Find(request.ReaderId);
            if (reader == null)
            {
                return Results.NotFound(); // Return 404 if the reader is not found
            }

            Item? item;

            // If ItemId is provided, find the specific item
            if (request.ItemId.HasValue)
            {
                item = db.Items.FirstOrDefault(i => i.ItemId == request.ItemId && i.Available == true);
            }
            else
            {
                // If only BookId is provided, find the first available item
                item = db.Items.FirstOrDefault(i => i.BookId == request.BookId && i.Available == true);
            }

            if (item == null)
            {
                return Results.NotFound("No available item found."); // Return 404 if no item is found
            }

            // Update the item's status to "unavailable" and link it to the reader
            item.Available = false;
            item.ReaderId = request.ReaderId;

            // Add a record to the history with status "issued" (StatusId = 2)
            var hist = new Hist
            {
                StatusId = 2,
                ItemId = item.ItemId,
                ReaderId = request.ReaderId,
                Comment = request.Comment
            };
            db.Hists.Add(hist);

            db.SaveChanges(); // Save the changes to the database
            return Results.Ok(item.ItemId); // Return the ID of the issued item
        }).RequireAuthorization();

        // Endpoint for returning a book or books to the library
        endpoints.MapPost("/business/return", (ReturnBookRequest request) =>
        {
            // If only ReaderId is provided, return all items linked to the reader
            if (request.ReaderId.HasValue && !request.ItemId.HasValue)
            {
                var items = db.Items.Where(i => i.ReaderId == request.ReaderId).ToList();
                if (!items.Any())
                {
                    return Results.NotFound("No items found for the specified reader."); // Return 404 if nothing is found
                }

                foreach (var item in items)
                {
                    // Update the item's status to "available" and clear ReaderId
                    item.Available = true;
                    item.ReaderId = null;

                    // Add a record to the history with status "returned" (StatusId = 3)
                    var hist = new Hist
                    {
                        StatusId = 3,
                        ItemId = item.ItemId,
                        ReaderId = request.ReaderId,
                        Comment = request.Comment
                    };
                    db.Hists.Add(hist);
                }

                db.SaveChanges(); // Save the changes to the database
                var itemIds = items.Select(i => i.ItemId).ToArray();
                return Results.Ok(itemIds); // Return IDs of the returned items
            }
            else
            {
                // If ItemId is provided, return the specific item
                var item = db.Items.Find(request.ItemId);
                if (item == null)
                {
                    return Results.NotFound(); // Return 404 if the item is not found
                }

                if (item.ReaderId == null)
                {
                    return Results.Ok("Action not performed as the item was not linked to a reader."); // Return message if the item was not linked
                }

                // Remember the current ReaderId before clearing it
                var currentReaderId = item.ReaderId;

                // Update the item's status to "available" and clear ReaderId
                item.Available = true;
                item.ReaderId = null;

                // Add a record to the history with status "returned" (StatusId = 3)
                var hist = new Hist
                {
                    StatusId = 3,
                    ItemId = item.ItemId,
                    ReaderId = request.ReaderId ?? currentReaderId,
                    Comment = request.Comment
                };
                db.Hists.Add(hist);

                db.SaveChanges(); // Save the changes to the database
                return Results.Ok(item.ItemId); // Return the ID of the returned item
            }
        }).RequireAuthorization();
    }
}

// Request model for adding book items
public class AddItemsRequest
{
    public int BookId { get; set; } // Book ID
    public string? Description { get; set; } // Item description (optional)
    public bool Available { get; set; } = true; // Whether the item is available
    public int Count { get; set; } = 1; // Number of items, default is 1
}

// Request model for issuing a book
public class GiveBookRequest
{
    public int? BookId { get; set; } // Book ID (optional, if ItemId is provided)
    public int? ItemId { get; set; } // Item ID (optional, if BookId is provided)
    public int ReaderId { get; set; } // Reader ID
    public string? Comment { get; set; } // Comment for the action (optional)
}

// Request model for returning a book
public class ReturnBookRequest
{
    public int? ItemId { get; set; } // Item ID (optional, if ReaderId is provided)
    public int? ReaderId { get; set; } // Reader ID (optional, if ItemId is provided)
    public string? Comment { get; set; } // Comment for the action (optional)
}
