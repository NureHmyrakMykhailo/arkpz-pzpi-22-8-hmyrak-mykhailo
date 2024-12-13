using API_NET6.Models;
using Microsoft.EntityFrameworkCore;


public static class ReadersEndpoints
{
    public static void MapReadersEndpoints(this IEndpointRouteBuilder endpoints, lib4Context db)
    {
        endpoints.MapGet("/readers", () => db.Readers
            .Select(r => new
            {
                r.ReaderId,
                r.Name,
                r.Class,
                r.StudentCard,
                r.Birthday,
                r.Phone,
                r.Email,
                r.Address
            }).ToList())
            .RequireAuthorization();

        endpoints.MapGet("/readers/{id}", (int id) =>
        {
            var reader = db.Readers
                .Include(r => r.Items)
                .ThenInclude(i => i.Book)
                .Include(r => r.Hists) // Include Hists related to the reader
                .Select(r => new
                {
                    r.ReaderId,
                    r.Name,
                    r.Class,
                    r.StudentCard,
                    r.Birthday,
                    r.Phone,
                    r.Email,
                    r.Address,
                    Items = r.Items.Select(i => new
                    {
                        i.ItemId,
                        i.Available,
                        i.Description,
                        BookTitle = i.Book.Title
                    }).ToList(),
                    Hists = r.Hists.Select(h => new
                    {
                        h.HistId,
                        h.Time,
                        h.StatusId,
                        h.ItemId,
                        h.Comment
                    }).ToList()
                })
                .FirstOrDefault(r => r.ReaderId == id);

            if (reader == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(reader);
        }).RequireAuthorization();


        endpoints.MapPost("/readers", (Reader reader) =>
        {
            db.Readers.Add(reader);
            db.SaveChanges();
            return reader;
        }).RequireAuthorization();

        endpoints.MapPut("/readers/{id}", (int id, Reader updatedReader) =>
        {
            var existingReader = db.Readers.Find(id);
            if (existingReader == null)
            {
                return Results.NotFound();
            }

            if (updatedReader.Name != null) existingReader.Name = updatedReader.Name;
            if (updatedReader.Class != null) existingReader.Class = updatedReader.Class;
            if (updatedReader.StudentCard != null) existingReader.StudentCard = updatedReader.StudentCard;
            if (updatedReader.Birthday != null) existingReader.Birthday = updatedReader.Birthday;
            if (updatedReader.Phone != null) existingReader.Phone = updatedReader.Phone;
            if (updatedReader.Email != null) existingReader.Email = updatedReader.Email;
            if (updatedReader.Address != null) existingReader.Address = updatedReader.Address;

            db.SaveChanges();
            return Results.Ok(existingReader);
        }).RequireAuthorization();

        endpoints.MapDelete("/readers/{id}", (int id) =>
        {
            var existingReader = db.Readers.Find(id);
            if (existingReader == null)
            {
                return Results.NotFound();
            }

            db.Readers.Remove(existingReader);
            db.SaveChanges();
            return Results.NoContent();
        }).RequireAuthorization();
    }
}
