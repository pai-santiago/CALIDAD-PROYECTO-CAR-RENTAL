using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using MySqlConnector;
using System.Data;

namespace CarRentalServices.Repositories
{
    public class CarRepository : ICarRepository
    {
        private const string SelectCars = """
            SELECT c.IdCar, c.Description, c.VinNumber, c.Model, c.BrandId,
                   COALESCE(NULLIF(c.Brand, ''), b.Name, '') AS Brand,
                   c.ColorId, COALESCE(NULLIF(c.Color, ''), clr.Name, '') AS Color,
                   c.CategoryId, COALESCE(NULLIF(c.Category, ''), cat.Description, cat.Label, '') AS Category,
                   c.Passengers
            FROM CarRental.Car AS c
            LEFT JOIN CarRental.Brand AS b ON b.IdBrand = c.BrandId
            LEFT JOIN CarRental.Color AS clr ON clr.IdColor = c.ColorId
            LEFT JOIN CarRental.Category AS cat ON cat.IdCategory = c.CategoryId
            """;

        private readonly IMySqlDataService _mySqlDataService;

        public CarRepository(IMySqlDataService mySqlDataService)
        {
            _mySqlDataService = mySqlDataService;
        }

        public IEnumerable<Car> GetCars()
        {
            var data = _mySqlDataService.ExecuteSql(SelectCars);
            return data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0
                ? MapCars(data.Tables[0])
                : [];
        }

        public Car? GetCarById(int id)
        {
            var data = _mySqlDataService.ExecuteSql($"{SelectCars} WHERE c.IdCar = @IdCar;", [
                new MySqlParameter("@IdCar", MySqlDbType.Int32) { Value = id }
            ]);
            return data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0
                ? MapCars(data.Tables[0]).FirstOrDefault()
                : null;
        }

        public bool InsertCar(Car car)
        {
            const string sql = """
                INSERT INTO CarRental.Car
                    (Description, VinNumber, Model, BrandId, ColorId, CategoryId, Passengers)
                VALUES (@Description, @VinNumber, @Model, @BrandId, @ColorId, @CategoryId, @Passengers);
                """;
            car.IdCar = checked((int)_mySqlDataService.ExecuteInsert(sql, CreateParameters(car)));
            return car.IdCar > 0;
        }

        public bool UpdateCar(Car car)
        {
            const string sql = """
                UPDATE CarRental.Car
                SET Description = @Description, VinNumber = @VinNumber, Model = @Model,
                    BrandId = @BrandId, ColorId = @ColorId, CategoryId = @CategoryId,
                    Passengers = @Passengers
                WHERE IdCar = @IdCar;
                """;
            return _mySqlDataService.ExecuteNonQuery(sql, CreateParameters(car, includeId: true)) > 0;
        }

        public bool DeleteCar(int id)
        {
            return _mySqlDataService.ExecuteNonQuery(
                "DELETE FROM CarRental.Car WHERE IdCar = @IdCar;",
                [new MySqlParameter("@IdCar", MySqlDbType.Int32) { Value = id }]) > 0;
        }

        private static List<Car> MapCars(DataTable dataTable)
        {
            return dataTable.Rows.Cast<DataRow>().Select(row => new Car
            {
                IdCar = Convert.ToInt32(row["IdCar"]),
                Description = Convert.ToString(row["Description"]) ?? string.Empty,
                VinNumber = Convert.ToString(row["VinNumber"]) ?? string.Empty,
                Model = Convert.ToInt32(row["Model"]),
                BrandId = Convert.ToInt32(row["BrandId"]),
                Brand = Convert.ToString(row["Brand"]) ?? string.Empty,
                ColorId = Convert.ToInt32(row["ColorId"]),
                Color = Convert.ToString(row["Color"]) ?? string.Empty,
                CategoryId = Convert.ToInt32(row["CategoryId"]),
                Category = Convert.ToString(row["Category"]) ?? string.Empty,
                Passengers = Convert.ToByte(row["Passengers"])
            }).ToList();
        }

        private static IEnumerable<MySqlParameter> CreateParameters(Car car, bool includeId = false)
        {
            var parameters = new List<MySqlParameter>
            {
                new("@Description", MySqlDbType.Text) { Value = car.Description },
                new("@VinNumber", MySqlDbType.VarChar) { Value = car.VinNumber },
                new("@Model", MySqlDbType.Int32) { Value = car.Model },
                new("@BrandId", MySqlDbType.Int32) { Value = car.BrandId },
                new("@ColorId", MySqlDbType.Int32) { Value = car.ColorId },
                new("@CategoryId", MySqlDbType.Int32) { Value = car.CategoryId },
                new("@Passengers", MySqlDbType.Byte) { Value = car.Passengers }
            };
            if (includeId)
            {
                parameters.Add(new MySqlParameter("@IdCar", MySqlDbType.Int32) { Value = car.IdCar });
            }
            return parameters;
        }
    }
}
