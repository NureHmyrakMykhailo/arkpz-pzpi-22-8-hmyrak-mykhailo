using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using API_NET6.Models;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace API_NET6
{
    public class Program
    {
        public static string LongerSecretKey = "f42a8390c349dff8305e31f55b640d2f9345cbaef22dddc204cd745feb6a5c23";
        public static string Issuer = "myapp";
        public static string Audience = "apps";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSwaggerConfiguration();
            builder.Services.AddAuthenticationConfiguration(Issuer, Audience, LongerSecretKey);
            builder.Services.AddAuthorization();

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            var app = builder.Build();

            var db = new lib4Context();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            //app.MapGet("/", () => "Hello World!");
            app.MapGet("/about", () => "Library V0.4 / NET 6");
            app.MapBooksEndpoints(db);
            app.MapItemsEndpoints(db);
            app.MapAuthEndpoints(Issuer, Audience, LongerSecretKey, db);
            app.MapAdminEndpoints();
            app.MapPersonsEndpoints(db);
            app.MapUsersEndpoints(db);
            app.MapReadersEndpoints(db);
            app.MapBusinessEndpoints(db);
            app.MapStatsEndpoints(db);


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API V 0.4");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.Run();
        }
    }
}
