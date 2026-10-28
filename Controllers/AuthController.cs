using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace  EmployeePortal.Api.Controllers;

public record LoginRequest(string Email, string Password);

public record LoginResponse(
    string AccessToken,
    DateTime ExpiresAtUtc
);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        // DEMO ONLY: Replace with database-backed user validation.
        if (request.Email != "admin" ||
            request.Password != "admin123")
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var jwt = _configuration.GetSection("Jwt");

        var secretKey = jwt["SecretKey"]!;
        var issuer = jwt["Issuer"]!;
        var audience = jwt["Audience"]!;

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey)
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var expiresAt = DateTime.UtcNow.AddMinutes(
            double.Parse(jwt["ExpiryMinutes"] ?? "15")
        );

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "1001"),
            new Claim(JwtRegisteredClaimNames.Email, request.Email),
            new Claim(ClaimTypes.Name, "Demo Admin"),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return Ok(new LoginResponse(accessToken, expiresAt));
    }
}