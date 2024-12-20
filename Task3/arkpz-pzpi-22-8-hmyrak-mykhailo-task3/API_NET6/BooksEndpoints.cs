using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using API_NET6.Models;
using Microsoft.EntityFrameworkCore;

public static class BooksEndpoints
{
    public static void MapBooksEndpoints(this IEndpointRouteBuilder endpoints, lib4Context db)
    {
        endpoints.MapGet("/books", () => db.Books
            .Select(b => new
            {
                b.BookId,
                b.Title,
                b.Isbn,
                b.Pages,
                b.Publish,
                b.CategoryId,
                b.Class,
                b.Lang,
                b.Year,
                ItemsCount = b.Items.Count(), // Count of items (copies) for each book
                AvailableItemsCount = b.Items.Count(i => i.Available) // Count of available items (copies) for each book
            }).ToList())
        .RequireAuthorization();




        endpoints.MapGet("/books/{id}", (int id) =>
        {
            var book = db.Books
                    .Include(b => b.Category)
                    .Include(b => b.Items)
                    .Include(b => b.BooksPeople)
                    .Where(b => b.BookId == id)
                    .Select(b => new
                    {
                        b.BookId,
                        b.Title,
                        b.Isbn,
                        b.Pages,
                        b.Publish,
                        b.CategoryId,
                        CategoryName = b.Category.Name,
                        b.Class,
                        b.Lang,
                        b.Year,
                        Items = b.Items.Select(i => new
                        {
                            i.ItemId,
                            i.ReaderId,
                            i.Available,
                            i.Description
                        }),

                        People = b.BooksPeople.Select(bp => new
                        {
                            bp.PersonId,
                            bp.Person.Name,
                            bp.RoleId
                        })
                    })
                    .FirstOrDefault();

            if (book == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(book);
        }).RequireAuthorization();



        endpoints.MapPost("/books", (Book book) =>
        {
            db.Books.Add(book);
            db.SaveChanges();
            return book;
        }).RequireAuthorization();

        endpoints.MapPut("/books/{id}", (int id, Book book) =>
        {
            var existingBook = db.Books.Find(id);
            if (existingBook == null)
            {
                return Results.NotFound();
            }

            if (book.Title != null) existingBook.Title = book.Title;
            if (book.Isbn != null) existingBook.Isbn = book.Isbn;
            if (book.Pages != null) existingBook.Pages = book.Pages;
            if (book.Publish != null) existingBook.Publish = book.Publish;
            if (book.CategoryId != null) existingBook.CategoryId = book.CategoryId;
            if (book.Class != null) existingBook.Class = book.Class;

            db.SaveChanges();
            return Results.Ok(existingBook);
        }).RequireAuthorization();

        endpoints.MapDelete("/books/{id}", (int id) =>
        {
            var existingBook = db.Books.Find(id);
            if (existingBook == null)
            {
                return Results.NotFound();
            }

            db.Books.Remove(existingBook);
            db.SaveChanges();
            return Results.NoContent();
        }).RequireAuthorization();

        endpoints.MapGet("/books/search", (string? title, string? isbn, int? pages, string? publish, int? categoryId) =>
        {
            var query = db.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(b => b.Title.Contains(title));
            }

            if (!string.IsNullOrWhiteSpace(isbn))
            {
                query = query.Where(b => b.Isbn == isbn);
            }

            if (pages.HasValue)
            {
                query = query.Where(b => b.Pages == pages);
            }

            if (!string.IsNullOrWhiteSpace(publish))
            {
                query = query.Where(b => b.Publish == publish);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId == categoryId);
            }

            return query.ToList();
        }).RequireAuthorization();
    }
}
