using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using MySqlConnector;
using System.Data;

namespace CarRentalServices.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly IMySqlDataService _mySqlDataService;

        public BrandRepository(IMySqlDataService mySqlDataService)
        {
            _mySqlDataService = mySqlDataService;
        }

        public IEnumerable<Brand> GetBrands()
        {
            var data = _mySqlDataService.ExecuteSql("SELECT IdBrand, Name FROM CarRental.Brand ORDER BY IdBrand;");
            return data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0
                ? MapBrands(data.Tables[0])
                : [];
        }

        public Brand? GetBrandById(int id)
        {
            var data = _mySqlDataService.ExecuteSql(
                "SELECT IdBrand, Name FROM CarRental.Brand WHERE IdBrand = @IdBrand;",
                [new MySqlParameter("@IdBrand", MySqlDbType.Int32) { Value = id }]);
            return data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0
                ? MapBrands(data.Tables[0]).FirstOrDefault()
                : null;
        }

        public bool InsertBrand(Brand brand)
        {
            brand.IdBrand = checked((int)_mySqlDataService.ExecuteInsert(
                "INSERT INTO CarRental.Brand (Name) VALUES (@Name);",
                [new MySqlParameter("@Name", MySqlDbType.VarChar, 100) { Value = brand.Name }]));
            return brand.IdBrand > 0;
        }

        public bool UpdateBrand(Brand brand)
        {
            return _mySqlDataService.ExecuteNonQuery(
                "UPDATE CarRental.Brand SET Name = @Name WHERE IdBrand = @IdBrand;",
                [
                    new MySqlParameter("@Name", MySqlDbType.VarChar, 100) { Value = brand.Name },
                    new MySqlParameter("@IdBrand", MySqlDbType.Int32) { Value = brand.IdBrand }
                ]) > 0;
        }

        public bool DeleteBrand(int id)
        {
            return _mySqlDataService.ExecuteNonQuery(
                "DELETE FROM CarRental.Brand WHERE IdBrand = @IdBrand;",
                [new MySqlParameter("@IdBrand", MySqlDbType.Int32) { Value = id }]) > 0;
        }

        private static List<Brand> MapBrands(DataTable dataTable)
        {
            return dataTable.Rows.Cast<DataRow>().Select(row => new Brand
            {
                IdBrand = Convert.ToInt32(row["IdBrand"]),
                Name = Convert.ToString(row["Name"]) ?? string.Empty
            }).ToList();
        }
    }
}
