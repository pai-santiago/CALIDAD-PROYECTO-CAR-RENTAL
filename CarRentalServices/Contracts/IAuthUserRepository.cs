using CarRentalServices.Entities;

namespace CarRentalServices.Contracts;

public interface IAuthUserRepository
{
    bool Exists(string normalizedUsername, string normalizedEmail);
    AuthUser? GetByUsername(string normalizedUsername);
    long Create(AuthUser user);
}
