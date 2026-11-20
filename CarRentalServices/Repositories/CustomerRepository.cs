using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using CarRentalServices.Utilities;
using MySqlConnector;
using System.Data;

namespace CarRentalServices.Repositories;

public class CustomerRepository(IMySqlDataService dataService) : ICustomerRepository
{
    public IEnumerable<Customer> GetCustomers()
    {
        var data = dataService.ExecSP("GetCustomers");
        return data.Tables.Count > 0 ? MapCustomers(data.Tables[0]) : [];
    }

    public Customer? GetCustomerById(int id)
    {
        var parameters = new[]
        {
            MySqlParameterBuilder.CreateParameter("@p_CustomerId", MySqlDbType.Int32, id)
        };
        var data = dataService.ExecSP("GetCustomerById", parameters);
        return data.Tables.Count > 0 ? MapCustomers(data.Tables[0]).FirstOrDefault() : null;
    }

    public bool InsertCustomer(Customer customer)
    {
        var data = dataService.ExecSP("InsertCustomer", CreateParameters(customer));
        if (data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
            customer.IdCustomer = Convert.ToInt32(data.Tables[0].Rows[0]["newCustomerId"]);

        return customer.IdCustomer > 0;
    }

    public bool UpdateCustomer(Customer customer)
    {
        var data = dataService.ExecSP("UpdateCustomer", CreateParameters(customer, includeId: true));
        return data.Tables.Count > 0
            && data.Tables[0].Rows.Count > 0
            && Convert.ToInt32(data.Tables[0].Rows[0]["RowsAffected"]) > 0;
    }

    private static List<Customer> MapCustomers(DataTable table)
    {
        return table.Rows.Cast<DataRow>()
            .Select(row => new Customer
            {
                IdCustomer = Convert.ToInt32(row["IdCustomer"]),
                FirstName = Convert.ToString(row["FirstName"]) ?? string.Empty,
                LastName = Convert.ToString(row["LastName"]) ?? string.Empty,
                SocialSecurityNumber = Convert.ToString(row["SSN"]) ?? string.Empty,
                Email = Convert.ToString(row["Email"]) ?? string.Empty,
                CountryName = Convert.ToString(row["Country"]) ?? string.Empty,
                CountryId = Convert.ToInt32(row["CountryId"]),
                MobilePhone = Convert.ToString(row["Mobile"]) ?? string.Empty,
                StateName = Convert.ToString(row["StateName"]) ?? string.Empty
            })
            .ToList();
    }

    private static IEnumerable<MySqlParameter> CreateParameters(Customer customer, bool includeId = false)
    {
        var parameters = new List<MySqlParameter>
        {
            MySqlParameterBuilder.CreateParameter("@p_FirstName", MySqlDbType.VarChar, 50, customer.FirstName),
            MySqlParameterBuilder.CreateParameter("@p_LastName", MySqlDbType.VarChar, 50, customer.LastName),
            MySqlParameterBuilder.CreateParameter("@p_SSN", MySqlDbType.VarChar, 50, customer.SocialSecurityNumber),
            MySqlParameterBuilder.CreateParameter("@p_Email", MySqlDbType.VarChar, 255, customer.Email),
            MySqlParameterBuilder.CreateParameter("@p_CountryId", MySqlDbType.Int32, customer.CountryId),
            MySqlParameterBuilder.CreateParameter("@p_Mobile", MySqlDbType.VarChar, 50, customer.MobilePhone),
            MySqlParameterBuilder.CreateParameter("@p_StateName", MySqlDbType.VarChar, 100, customer.StateName)
        };

        if (includeId)
            parameters.Add(MySqlParameterBuilder.CreateParameter("@p_CustomerId", MySqlDbType.Int32, customer.IdCustomer));

        return parameters;
    }
}
