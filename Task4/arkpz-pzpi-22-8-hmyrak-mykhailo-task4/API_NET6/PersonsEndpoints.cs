using API_NET6.Models;
using Microsoft.EntityFrameworkCore;

public static class PersonsEndpoints
{
    public static void MapPersonsEndpoints(this IEndpointRouteBuilder endpoints, libraryContext db)
    {
        endpoints.MapGet("/persons", () => db.Persons)
            .RequireAuthorization();

        endpoints.MapGet("/persons/{id}", (int id) =>
        {
            var person = db.Persons
                    .Include(p => p.BooksPeople)
                    .Where(p => p.PersonId == id)
                    .Select(p => new
                    {
                        p.PersonId,
                        p.Name,
                        p.DateOfBirth,
                        p.DateOfDeath,
                        p.Country,
                        p.IsReal,
                        Books = p.BooksPeople.Select(bp => new
                        {
                            bp.BookId,
                            bp.Book.Title,
                            bp.RoleId
                        })
                    })
                    .FirstOrDefault();

            if (person == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(person);
        }).RequireAuthorization();

        endpoints.MapPost("/persons", (Person person) =>
        {
            db.Persons.Add(person);
            db.SaveChanges();
            return person;
        }).RequireAuthorization();

        endpoints.MapPut("/persons/{id}", (int id, Person person) =>
        {
            var existingPerson = db.Persons.Find(id);
            if (existingPerson == null)
            {
                return Results.NotFound();
            }

            if (person.Name != null) existingPerson.Name = person.Name;
            if (person.DateOfBirth != null) existingPerson.DateOfBirth = person.DateOfBirth;
            if (person.DateOfDeath != null) existingPerson.DateOfDeath = person.DateOfDeath;
            if (person.Country != null) existingPerson.Country = person.Country;
            if (person.IsReal.HasValue) existingPerson.IsReal = person.IsReal;

            db.SaveChanges();
            return Results.Ok(existingPerson);
        }).RequireAuthorization();

        endpoints.MapDelete("/persons/{id}", (int id) =>
        {
            var existingPerson = db.Persons.Find(id);
            if (existingPerson == null)
            {
                return Results.NotFound();
            }

            db.Persons.Remove(existingPerson);
            db.SaveChanges();
            return Results.NoContent();
        }).RequireAuthorization();

        endpoints.MapGet("/persons/search", (string? name, DateTime? dateOfBirth, DateTime? dateOfDeath, string? country, bool? isReal) =>
        {
            var query = db.Persons.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            if (dateOfBirth.HasValue)
            {
                query = query.Where(p => p.DateOfBirth == dateOfBirth);
            }

            if (dateOfDeath.HasValue)
            {
                query = query.Where(p => p.DateOfDeath == dateOfDeath);
            }

            if (!string.IsNullOrWhiteSpace(country))
            {
                query = query.Where(p => p.Country == country);
            }

            if (isReal.HasValue)
            {
                query = query.Where(p => p.IsReal == isReal);
            }

            return query.ToList();
        }).RequireAuthorization();
    }
}
