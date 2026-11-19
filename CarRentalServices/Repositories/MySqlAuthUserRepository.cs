using System.Data;
using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using CarRentalServices.Services;
using MySqlConnector;

namespace CarRentalServices.Repositories;

public sealed class MySqlAuthUserRepository(IMySqlDataService mysql) : IAuthUserRepository
{
    private const string SelectUser = """
        SELECT u.Id, u.Username, u.NormalizedUsername, u.Email, u.NormalizedEmail,
               u.PasswordHash, r.Name AS Role, u.CreatedAtUtc
        FROM auth_users u
        INNER JOIN auth_roles r ON r.Id = u.RoleId
        """;

    public bool Exists(string normalizedUsername, string normalizedEmail)
    {
        var result = mysql.ExecuteSql(
            "SELECT 1 FROM auth_users WHERE NormalizedUsername = @username OR NormalizedEmail = @email LIMIT 1;",
            [new MySqlParameter("@username", normalizedUsername), new MySqlParameter("@email", normalizedEmail)]);
        return result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0;
    }

    public AuthUser? GetByUsername(string normalizedUsername)
    {
        var result = mysql.ExecuteSql(
            $"{SelectUser} WHERE u.NormalizedUsername = @username LIMIT 1;",
            [new MySqlParameter("@username", normalizedUsername)]);
        if (result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
            return null;

        var row = result.Tables[0].Rows[0];
        return new AuthUser
        {
            Id = Convert.ToInt64(row["Id"]),
            Username = Convert.ToString(row["Username"])!,
            NormalizedUsername = Convert.ToString(row["NormalizedUsername"])!,
            Email = Convert.ToString(row["Email"])!,
            NormalizedEmail = Convert.ToString(row["NormalizedEmail"])!,
            PasswordHash = Convert.ToString(row["PasswordHash"])!,
            Role = Convert.ToString(row["Role"])!,
            CreatedAtUtc = DateTime.SpecifyKind(Convert.ToDateTime(row["CreatedAtUtc"]), DateTimeKind.Utc)
        };
    }

    public long Create(AuthUser user)
    {
        return mysql.ExecuteInsert("""
            INSERT INTO auth_users
                (Username, NormalizedUsername, Email, NormalizedEmail, PasswordHash, RoleId, CreatedAtUtc)
            VALUES
                (@username, @normalizedUsername, @email, @normalizedEmail, @passwordHash,
                 (SELECT Id FROM auth_roles WHERE Name = 'User'), @createdAtUtc);
            """, [
                new MySqlParameter("@username", user.Username),
                new MySqlParameter("@normalizedUsername", user.NormalizedUsername),
                new MySqlParameter("@email", user.Email),
                new MySqlParameter("@normalizedEmail", user.NormalizedEmail),
                new MySqlParameter("@passwordHash", user.PasswordHash),
                new MySqlParameter("@createdAtUtc", user.CreatedAtUtc)
            ]);
    }
}
