using System.Security.Claims;
using CarRental.Security;
using CarRental.Models;
using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace CarRental.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IAuthUserRepository users,
    IPasswordHasher<AuthUser> passwords,
    JwtTokenService tokens) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        var username = request.Username.Trim();
        var email = request.Email.Trim();
        var normalizedUsername = username.ToUpperInvariant();
        var normalizedEmail = email.ToUpperInvariant();

        if (users.Exists(normalizedUsername, normalizedEmail))
            return Conflict(new { message = "El usuario o correo ya está registrado." });

        var user = new AuthUser
        {
            Username = username,
            NormalizedUsername = normalizedUsername,
            Email = email,
            NormalizedEmail = normalizedEmail,
            Role = "User",
            CreatedAtUtc = DateTime.UtcNow
        };
        user.PasswordHash = passwords.HashPassword(user, request.Password);

        try
        {
            user.Id = users.Create(user);
        }
        catch (MySqlException exception) when (exception.Number == 1062)
        {
            return Conflict(new { message = "El usuario o correo ya está registrado." });
        }

        return Ok(new
        {
            message = "Usuario registrado correctamente.",
            user = new { user.Id, user.Username, user.Email, user.Role, user.CreatedAtUtc }
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var user = users.GetByUsername(request.Username.Trim().ToUpperInvariant());
        if (user is null || passwords.VerifyHashedPassword(user, user.PasswordHash, request.Password)
            == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Usuario o contraseña incorrectos." });

        var issued = tokens.Create(user);
        return Ok(new
        {
            accessToken = issued.Token,
            tokenType = "Bearer",
            expiresAtUtc = issued.ExpiresAtUtc,
            user = new { user.Username, user.Email, user.Role }
        });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new
    {
        username = User.FindFirstValue("sub"),
        email = User.FindFirstValue("email"),
        role = User.FindFirstValue(ClaimTypes.Role)
    });
}
