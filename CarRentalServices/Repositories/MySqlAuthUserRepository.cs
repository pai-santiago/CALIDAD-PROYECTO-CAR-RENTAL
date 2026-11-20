using System.Data;
using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using CarRentalServices.Utilities;
using MySqlConnector;

namespace CarRentalServices.Repositories
{
    public class MySqlAuthUserRepository : IAuthUserRepository
    {
        private readonly IMySqlDataService _dataService;

        public MySqlAuthUserRepository(IMySqlDataService dataService)
        {
            _dataService = dataService;
        }

        public AuthUser? GetByUsername(string normalizedUsername)
        {
            var parameters = new[]
            {
                MySqlParameterBuilder.CreateParameter("@p_NormalizedUsername", MySqlDbType.VarChar, 60, normalizedUsername)
            };
            var data = _dataService.ExecSP("GetAuthUserByUsername", parameters);
            if (data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0)
                return null;

            var row = data.Tables[0].Rows[0];
            return new AuthUser
            {
                Id = Convert.ToInt64(row["Id"]),
                Username = Convert.ToString(row["Username"]) ?? string.Empty,
                NormalizedUsername = Convert.ToString(row["NormalizedUsername"]) ?? string.Empty,
                Email = Convert.ToString(row["Email"]) ?? string.Empty,
                NormalizedEmail = Convert.ToString(row["NormalizedEmail"]) ?? string.Empty,
                PasswordHash = Convert.ToString(row["PasswordHash"]) ?? string.Empty,
                Role = Convert.ToString(row["Role"]) ?? "User",
                CreatedAtUtc = DateTime.SpecifyKind(Convert.ToDateTime(row["CreatedAtUtc"]), DateTimeKind.Utc)
            };
        }

        public long Create(AuthUser user)
        {
            var parameters = new[]
            {
                MySqlParameterBuilder.CreateParameter("@p_Username", MySqlDbType.VarChar, 60, user.Username),
                MySqlParameterBuilder.CreateParameter("@p_NormalizedUsername", MySqlDbType.VarChar, 60, user.NormalizedUsername),
                MySqlParameterBuilder.CreateParameter("@p_Email", MySqlDbType.VarChar, 254, user.Email),
                MySqlParameterBuilder.CreateParameter("@p_NormalizedEmail", MySqlDbType.VarChar, 254, user.NormalizedEmail),
                MySqlParameterBuilder.CreateParameter("@p_PasswordHash", MySqlDbType.VarChar, 512, user.PasswordHash),
                MySqlParameterBuilder.CreateParameter("@p_CreatedAtUtc", MySqlDbType.DateTime, user.CreatedAtUtc)
            };
            var data = _dataService.ExecSP("CreateAuthUser", parameters);
            if (data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
                return Convert.ToInt64(data.Tables[0].Rows[0]["newUserId"]);

            throw new InvalidOperationException("The user was not created.");
        }
    }
}
