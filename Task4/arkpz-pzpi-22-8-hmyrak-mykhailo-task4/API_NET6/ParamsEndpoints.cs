using API_NET6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

public static class ParamsEndpoints
{
    public static void MapParamsEndpoints(this IEndpointRouteBuilder endpoints, libraryContext db)
    {


        endpoints.MapGet("/params", [Authorize] () =>
        {
            var param = db.Params.Find(1);
            if (param == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(param);
        });


        endpoints.MapPut("/params", [Authorize(Roles = "Admin")] (Param param) =>
        {
            var existingParam = db.Params.Find(1);
            if (existingParam == null)
            {
                return Results.NotFound();
            }

            if (param.TempMax.HasValue) existingParam.TempMax = param.TempMax;
            if (param.TempMin.HasValue) existingParam.TempMin = param.TempMin;
            if (param.WetMax.HasValue) existingParam.WetMax = param.WetMax;
            if (param.WetMin.HasValue) existingParam.WetMin = param.WetMin;

            db.SaveChanges();
            return Results.Ok(existingParam);
        }).RequireAuthorization();

    }
}
