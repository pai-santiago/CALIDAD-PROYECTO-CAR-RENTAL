using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarRentalServices.Entities;
using Microsoft.IdentityModel.Tokens;

namespace CarRental.Security;

public sealed class JwtTokenService(string key, string issuer, string audience, int expirationMinutes)
{
    public (string Token, DateTime ExpiresAtUtc) Create(AuthUser user)
    {
        var expires = DateTime.UtcNow.AddMinutes(expirationMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer, audience, claims, DateTime.UtcNow, expires, credentials);
        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
