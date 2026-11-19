using System.ComponentModel.DataAnnotations;

namespace CarRental.Models;

public sealed class RegisterRequest
{
    [Required, StringLength(60, MinimumLength = 3)]
    [RegularExpression("^[a-zA-Z0-9._-]+$")]
    public string Username { get; init; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required, StringLength(128, MinimumLength = 10)]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).+$")]
    public string Password { get; init; } = string.Empty;
}

public sealed class LoginRequest
{
    [Required, StringLength(60, MinimumLength = 3)]
    public string Username { get; init; } = string.Empty;

    [Required, StringLength(128, MinimumLength = 1)]
    public string Password { get; init; } = string.Empty;
}
