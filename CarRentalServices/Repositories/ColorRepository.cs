using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using CarRentalServices.Enums;
using CarRentalServices.Services;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalServices.Repositories
{
    public class ColorRepository : IColorRepository
    {
        private readonly IMySqlDataService _mySqlDataService;

        public ColorRepository(IMySqlDataService mySqlDataService) { 
            _mySqlDataService = mySqlDataService;   
        }

        public bool DeleteColor(int id)
        {
            return _mySqlDataService.ExecuteNonQuery(
                "DELETE FROM CarRental.Color WHERE IdColor = @IdColor;",
                [new MySqlParameter("@IdColor", MySqlDbType.Int32) { Value = id }]) > 0;
        }

        public IEnumerable<Color> GetColors()
        {
            var data = _mySqlDataService.ExecuteSql("SELECT * FROM CarRental.Color;");
            if (data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
            {
                return MapColors(data.Tables[0]);
            }
            return [];
        }

        public Color? GetColor(int id)
        {
            var data = _mySqlDataService.ExecuteSql(
                "SELECT * FROM CarRental.Color WHERE IdColor = @IdColor;",
                [new MySqlParameter("@IdColor", MySqlDbType.Int32) { Value = id }]);
            if (data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
            {
                return MapColors(data.Tables[0]).FirstOrDefault();
            }
            return null;
        }

        public int InsertColor(Color Color)
        {
            const string sql = "INSERT INTO CarRental.Color (Name) VALUES (@Name);";
            Color.IdColor = checked((int)_mySqlDataService.ExecuteInsert(sql, [
                new MySqlParameter("@Name", MySqlDbType.VarChar, 50) { Value = Color.Name }
            ]));
            return Color.IdColor;
        }

        public bool UpdateColor(Color color)
        {
            const string sql = "UPDATE CarRental.Color SET Name = @Name WHERE IdColor = @IdColor;";
            return _mySqlDataService.ExecuteNonQuery(sql, [
                new MySqlParameter("@Name", MySqlDbType.VarChar, 50) { Value = color.Name },
                new MySqlParameter("@IdColor", MySqlDbType.Int32) { Value = color.IdColor }
            ]) > 0;
        }


        #region "Private Methods"
        private static List<Color> MapColors(DataTable dataTable)
        {
            var lstColors = new List<Color>();
            foreach (DataRow row in dataTable.Rows)
            {
                var color = new Color
                {
                    IdColor = Convert.ToInt32(row["IdColor"]),
                    Name = (string)row["Name"]
                };
                lstColors.Add(color);
            }

            return lstColors;
        }
        #endregion
    }
}
