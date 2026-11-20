using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using CarRentalServices.Utilities;
using MySqlConnector;
using System.Data;

namespace CarRentalServices.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly IMySqlDataService _dataService;

        public CountryRepository(IMySqlDataService dataService)
        {
            _dataService = dataService;
        }

        public IEnumerable<Country> GetCountries()
        {
            var data = _dataService.ExecSP("GetCountries");
            return data.Tables.Count > 0 ? MapCountries(data.Tables[0]) : [];
        }

        public Country? GetCountryById(int id)
        {
            var parameters = new[]
            {
                MySqlParameterBuilder.CreateParameter("@p_CountryId", MySqlDbType.Int32, id)
            };
            var data = _dataService.ExecSP("GetCountryById", parameters);
            return data.Tables.Count > 0 ? MapCountries(data.Tables[0]).FirstOrDefault() : null;
        }

        public bool InsertCountry(Country country)
        {
            var parameters = new[]
            {
                MySqlParameterBuilder.CreateParameter("@p_CountryName", MySqlDbType.VarChar, 100, country.CountryName)
            };
            var result = _dataService.ExecSP("InsertCountry", parameters);
            if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
            {
                country.IdCountry = Convert.ToInt32(result.Tables[0].Rows[0]["newCountryId"]);
            }

            return country.IdCountry > 0;
        }

        public bool UpdateCountry(Country country)
        {
            var parameters = new[]
            {
                MySqlParameterBuilder.CreateParameter("@p_CountryName", MySqlDbType.VarChar, 100, country.CountryName),
                MySqlParameterBuilder.CreateParameter("@p_CountryId", MySqlDbType.Int32, country.IdCountry)
            };
            var result = _dataService.ExecSP("UpdateCountry", parameters);
            return result.Tables.Count > 0
                && result.Tables[0].Rows.Count > 0
                && Convert.ToInt32(result.Tables[0].Rows[0]["RowsAffected"]) > 0;
        }

        private static List<Country> MapCountries(DataTable table)
        {
            return table.Rows.Cast<DataRow>()
                .Select(row => new Country
                {
                    IdCountry = Convert.ToInt32(row["IdCountry"]),
                    CountryName = Convert.ToString(row["CountryName"]) ?? string.Empty
                })
                .ToList();
        }
    }
}
