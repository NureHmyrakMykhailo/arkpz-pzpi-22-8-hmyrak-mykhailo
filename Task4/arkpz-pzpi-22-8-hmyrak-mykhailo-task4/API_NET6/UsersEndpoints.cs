using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using API_NET6.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder endpoints, libraryContext db)
    {
        // Get list of users
        endpoints.MapGet("/users", [Authorize(Roles = "Admin")] () => db.Users.ToList())
            .RequireAuthorization();

        // Edit user
        endpoints.MapPut("/users/{id}", [Authorize(Roles = "Admin")] (int id, User user) =>
        {
            var existingUser = db.Users.Find(id);
            if (existingUser == null)
            {
                return Results.NotFound();
            }

            if (user.Login != null) existingUser.Login = user.Login;
            if (user.Email != null) existingUser.Email = user.Email;
            if (user.Role != null) existingUser.Role = user.Role;
            if (user.PasswordHash != null) existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            existingUser.UpdatedAt = DateTime.UtcNow;

            db.SaveChanges();
            return Results.Ok(existingUser);
        }).RequireAuthorization();

        // Delete user
        endpoints.MapDelete("/users/{id}", [Authorize(Roles = "Admin")] (int id) =>
        {
            var existingUser = db.Users.Find(id);
            if (existingUser == null)
            {
                return Results.NotFound();
            }

            db.Users.Remove(existingUser);
            db.SaveChanges();
            return Results.NoContent();
        }).RequireAuthorization();
    }
}
