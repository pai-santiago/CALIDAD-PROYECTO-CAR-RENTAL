using System.Security.Claims;
using CarRental.Models;
using CarRental.Security;
using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthUserRepository _users;
        private readonly IPasswordHasher<AuthUser> _passwords;
        private readonly JwtTokenService _tokens;

        public AuthController(
            IAuthUserRepository users,
            IPasswordHasher<AuthUser> passwords,
            JwtTokenService tokens)
        {
            _users = users;
            _passwords = passwords;
            _tokens = tokens;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var username = request.Username.Trim();
            var email = request.Email.Trim();
            var normalizedUsername = username.ToUpperInvariant();

            var user = new AuthUser
            {
                Username = username,
                NormalizedUsername = normalizedUsername,
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                Role = "User",
                CreatedAtUtc = DateTime.UtcNow
            };
            user.PasswordHash = _passwords.HashPassword(user, request.Password);

            try
            {
                user.Id = _users.Create(user);
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
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var username = request.Username.Trim().ToUpperInvariant();
            var user = _users.GetByUsername(username);
            if (user is null || _passwords.VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Usuario o contraseña incorrectos." });

            var issued = _tokens.Create(user);
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
        public IActionResult Me()
        {
            return Ok(new
            {
                username = User.FindFirstValue("sub"),
                email = User.FindFirstValue("email"),
                role = User.FindFirstValue(ClaimTypes.Role)
            });
        }
    }
}
