using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using MySqlConnector;
using System.Data;

namespace CarRentalServices.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly IMySqlDataService _mySqlDataService;

        public LocationRepository(IMySqlDataService mySqlDataService)
        {
            _mySqlDataService = mySqlDataService;
        }

        public IEnumerable<Location> GetLocations()
        {
            var data = _mySqlDataService.ExecuteSql("SELECT * FROM CarRental.Location ORDER BY Id;");
            return data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0
                ? MapLocations(data.Tables[0])
                : [];
        }

        public Location? GetLocationById(int id)
        {
            var data = _mySqlDataService.ExecuteSql(
                "SELECT * FROM CarRental.Location WHERE Id = @Id;",
                [new MySqlParameter("@Id", MySqlDbType.Int32) { Value = id }]);
            return data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0
                ? MapLocations(data.Tables[0]).FirstOrDefault()
                : null;
        }

        public bool InsertLocation(Location location)
        {
            const string sql = """
                INSERT INTO CarRental.Location
                    (StreetName, StreetNumber, City, State, CountryName, CountryId)
                VALUES
                    (@StreetName, @StreetNumber, @City, @State, @CountryName, @CountryId);
                """;
            location.Id = checked((int)_mySqlDataService.ExecuteInsert(sql, CreateParameters(location)));
            return location.Id > 0;
        }

        public bool UpdateLocation(Location location)
        {
            const string sql = """
                UPDATE CarRental.Location
                SET StreetName = @StreetName, StreetNumber = @StreetNumber,
                    City = @City, State = @State, CountryName = @CountryName,
                    CountryId = @CountryId
                WHERE Id = @Id;
                """;
            return _mySqlDataService.ExecuteNonQuery(sql, CreateParameters(location, includeId: true)) > 0;
        }

        public bool DeleteLocation(int id)
        {
            return _mySqlDataService.ExecuteNonQuery(
                "DELETE FROM CarRental.Location WHERE Id = @Id;",
                [new MySqlParameter("@Id", MySqlDbType.Int32) { Value = id }]) > 0;
        }

        private static List<Location> MapLocations(DataTable dataTable)
        {
            return dataTable.Rows.Cast<DataRow>().Select(row => new Location
            {
                Id = Convert.ToInt32(row["Id"]),
                StreetName = Convert.ToString(row["StreetName"]) ?? string.Empty,
                StreetNumber = Convert.ToString(row["StreetNumber"]) ?? string.Empty,
                City = Convert.ToString(row["City"]) ?? string.Empty,
                State = Convert.ToString(row["State"]) ?? string.Empty,
                CountryName = Convert.ToString(row["CountryName"]) ?? string.Empty,
                CountryId = Convert.ToInt32(row["CountryId"])
            }).ToList();
        }

        private static IEnumerable<MySqlParameter> CreateParameters(Location location, bool includeId = false)
        {
            var parameters = new List<MySqlParameter>
            {
                new("@StreetName", MySqlDbType.VarChar) { Value = location.StreetName ?? string.Empty },
                new("@StreetNumber", MySqlDbType.VarChar) { Value = location.StreetNumber ?? string.Empty },
                new("@City", MySqlDbType.VarChar) { Value = location.City ?? string.Empty },
                new("@State", MySqlDbType.VarChar) { Value = location.State ?? string.Empty },
                new("@CountryName", MySqlDbType.VarChar) { Value = location.CountryName ?? string.Empty },
                new("@CountryId", MySqlDbType.Int32) { Value = location.CountryId }
            };
            if (includeId)
            {
                parameters.Add(new MySqlParameter("@Id", MySqlDbType.Int32) { Value = location.Id });
            }
            return parameters;
        }
    }
}
