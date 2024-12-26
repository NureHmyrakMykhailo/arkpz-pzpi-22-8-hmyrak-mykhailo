using Microsoft.AspNetCore.Authorization;

namespace API_NET6
{
    public static class AdminEndpoints
    {
        public static void MapAdminEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/user", [Authorize] () => Results.Ok("User page"));

            endpoints.MapGet("/free", () => Results.Ok("Free page"));

            endpoints.MapGet("/admin", [Authorize(Roles = "Admin")] () => Results.Ok("Admin page"));

        }

    }
}
