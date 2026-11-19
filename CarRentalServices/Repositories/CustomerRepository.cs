using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using MySqlConnector;
using System.Data;

namespace CarRentalServices.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMySqlDataService _mySqlDataService;

        public CustomerRepository(IMySqlDataService mySqlDataService)
        {
            _mySqlDataService = mySqlDataService;
        }

        public IEnumerable<Customer> GetCustomers()
        {
            var data = _mySqlDataService.ExecuteSql("SELECT * FROM CarRental.Customer ORDER BY IdCustomer;");
            return data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0
                ? MapCustomers(data.Tables[0])
                : [];
        }

        public Customer? GetCustomerById(int id)
        {
            var data = _mySqlDataService.ExecuteSql(
                "SELECT * FROM CarRental.Customer WHERE IdCustomer = @IdCustomer;",
                [new MySqlParameter("@IdCustomer", MySqlDbType.Int32) { Value = id }]);
            return data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0
                ? MapCustomers(data.Tables[0]).FirstOrDefault()
                : null;
        }

        public bool InsertCustomer(Customer customer)
        {
            const string sql = """
                INSERT INTO CarRental.Customer
                    (FirstName, LastName, SocialSecurityNumber, Email, CountryName, CountryId, MobilePhone, StateName)
                VALUES
                    (@FirstName, @LastName, @SocialSecurityNumber, @Email, @CountryName, @CountryId, @MobilePhone, @StateName);
                """;
            customer.IdCustomer = checked((int)_mySqlDataService.ExecuteInsert(sql, CreateParameters(customer)));
            return customer.IdCustomer > 0;
        }

        public bool UpdateCustomer(Customer customer)
        {
            const string sql = """
                UPDATE CarRental.Customer
                SET FirstName = @FirstName, LastName = @LastName,
                    SocialSecurityNumber = @SocialSecurityNumber, Email = @Email,
                    CountryName = @CountryName, CountryId = @CountryId,
                    MobilePhone = @MobilePhone, StateName = @StateName
                WHERE IdCustomer = @IdCustomer;
                """;
            return _mySqlDataService.ExecuteNonQuery(sql, CreateParameters(customer, includeId: true)) > 0;
        }

        public bool DeleteCustomer(int id)
        {
            return _mySqlDataService.ExecuteNonQuery(
                "DELETE FROM CarRental.Customer WHERE IdCustomer = @IdCustomer;",
                [new MySqlParameter("@IdCustomer", MySqlDbType.Int32) { Value = id }]) > 0;
        }

        private static List<Customer> MapCustomers(DataTable dataTable)
        {
            return dataTable.Rows.Cast<DataRow>().Select(row => new Customer
            {
                IdCustomer = Convert.ToInt32(row["IdCustomer"]),
                FirstName = Convert.ToString(row["FirstName"]) ?? string.Empty,
                LastName = Convert.ToString(row["LastName"]) ?? string.Empty,
                SocialSecurityNumber = Convert.ToString(row["SocialSecurityNumber"]) ?? string.Empty,
                Email = Convert.ToString(row["Email"]) ?? string.Empty,
                CountryName = Convert.ToString(row["CountryName"]) ?? string.Empty,
                CountryId = Convert.ToInt32(row["CountryId"]),
                MobilePhone = Convert.ToString(row["MobilePhone"]) ?? string.Empty,
                StateName = Convert.ToString(row["StateName"]) ?? string.Empty
            }).ToList();
        }

        private static IEnumerable<MySqlParameter> CreateParameters(Customer customer, bool includeId = false)
        {
            var parameters = new List<MySqlParameter>
            {
                new("@FirstName", MySqlDbType.VarChar) { Value = customer.FirstName ?? string.Empty },
                new("@LastName", MySqlDbType.VarChar) { Value = customer.LastName ?? string.Empty },
                new("@SocialSecurityNumber", MySqlDbType.VarChar) { Value = customer.SocialSecurityNumber ?? string.Empty },
                new("@Email", MySqlDbType.VarChar) { Value = customer.Email ?? string.Empty },
                new("@CountryName", MySqlDbType.VarChar) { Value = customer.CountryName ?? string.Empty },
                new("@CountryId", MySqlDbType.Int32) { Value = customer.CountryId },
                new("@MobilePhone", MySqlDbType.VarChar) { Value = customer.MobilePhone ?? string.Empty },
                new("@StateName", MySqlDbType.VarChar) { Value = customer.StateName ?? string.Empty }
            };
            if (includeId)
            {
                parameters.Add(new MySqlParameter("@IdCustomer", MySqlDbType.Int32) { Value = customer.IdCustomer });
            }
            return parameters;
        }
    }
}
