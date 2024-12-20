using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API_NET6.Models;
using BCrypt.Net;

namespace API_NET6
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder endpoints, string issuer, string audience, string secretKey, lib4Context db)
        {
            endpoints.MapPost("/auth/login", (LoginRequest loginRequest) =>
            {
                // Check if the user exists in the database
                var user = db.Users.FirstOrDefault(u => u.Login == loginRequest.Username);
                if (user != null && BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
                {
                    var claims = new[]
                    {
                            new Claim(ClaimTypes.Name, user.Login),
                            new Claim(ClaimTypes.Role, user.Role)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: issuer,
                        audience: audience,
                        claims: claims,
                        expires: DateTime.Now.AddHours(2),
                        signingCredentials: creds);

                    return Results.Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token)
                    });
                }

                return Results.Unauthorized();
            });

            endpoints.MapPost("/auth/register", (RegisterRequest registerRequest) =>
            {
                if (string.IsNullOrEmpty(registerRequest.Username) || string.IsNullOrEmpty(registerRequest.Password) || string.IsNullOrEmpty(registerRequest.Email))
                {
                    return Results.BadRequest("Username, password and email are required");
                }

                // Check if the user already exists
                var existingUser = db.Users.FirstOrDefault(u => u.Login == registerRequest.Username);
                if (existingUser != null)
                {
                    return Results.BadRequest("User already exists");
                }

                var newUser = new User
                {
                    Login = registerRequest.Username,
                    Email = registerRequest.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password)
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                return Results.Ok(new { userId = newUser.UserId });
            });
        }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

}
