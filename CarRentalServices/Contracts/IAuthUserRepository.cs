using CarRentalServices.Entities;

namespace CarRentalServices.Contracts;

public interface IAuthUserRepository
{
    AuthUser? GetByUsername(string normalizedUsername);
    long Create(AuthUser user);
}
