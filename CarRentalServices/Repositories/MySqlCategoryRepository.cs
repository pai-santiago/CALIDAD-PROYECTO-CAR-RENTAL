using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using CarRentalServices.Enums;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using System.Data;

namespace CarRentalServices.Repositories
{
    public class MySqlCategoryRepository : ICategoryRepository
    {

        private readonly IMySqlDataService _mySqlDataService;

        public MySqlCategoryRepository(IMySqlDataService mySqlDataService)
        {
            _mySqlDataService = mySqlDataService;
        }

        #region public
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Category> GetCategories()
        {
            var data = _mySqlDataService.ExecuteSql("SELECT * FROM CarRental.Category;");
            List<Category> lstCategories = [];
            if (data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
            {
                lstCategories = MapCategories(data.Tables[0]);
            }
            return lstCategories;
        }

        public Category? GetCategoryById(int id)
        {
            var data = _mySqlDataService.ExecuteSql(
                "SELECT * FROM CarRental.Category WHERE IdCategory = @IdCategory;",
                [new MySqlParameter("@IdCategory", MySqlDbType.Int32) { Value = id }]);
            if (data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
            {
                return MapCategories(data.Tables[0]).FirstOrDefault();
            }
            return null;
        }

        public bool InsertCategory(Category category)
        {
            const string sql = "INSERT INTO CarRental.Category (Label, Description) VALUES (@Label, @Description);";
            IEnumerable<MySqlParameter> parameters = CreateParametersMySql(category, ActionType.Insert);
            category.IdCategory = checked((int)_mySqlDataService.ExecuteInsert(sql, parameters));
            return true;
        }

        public bool UpdateCategory(Category category)
        {
            const string sql = """
                UPDATE CarRental.Category
                SET Label = @Label, Description = @Description
                WHERE IdCategory = @IdCategory;
                """;
            return _mySqlDataService.ExecuteNonQuery(sql, [
                new MySqlParameter("@Label", MySqlDbType.VarChar, 100) { Value = category.Label ?? (object)DBNull.Value },
                new MySqlParameter("@Description", MySqlDbType.Text) { Value = category.Description ?? (object)DBNull.Value },
                new MySqlParameter("@IdCategory", MySqlDbType.Int32) { Value = category.IdCategory }
            ]) > 0;
        }

        public bool DeleteCategory(int id)
        {
            return _mySqlDataService.ExecuteNonQuery(
                "DELETE FROM CarRental.Category WHERE IdCategory = @IdCategory;",
                [new MySqlParameter("@IdCategory", MySqlDbType.Int32) { Value = id }]) > 0;
        }
        #endregion

        #region private
        private static List<Category> MapCategories(DataTable dataTable)
        {
            var lstCategories = new List<Category>();
            foreach (DataRow row in dataTable.Rows)
            {
                var category = new Category
                {
                    IdCategory = Convert.ToInt32(row["IdCategory"]),
                    Label = row.IsNull("Label") ? null : Convert.ToString(row["Label"]),
                    Description = row.IsNull("Description") ? null : Convert.ToString(row["Description"])
                };
                lstCategories.Add(category);
            }

            return lstCategories;
        }

        private static IEnumerable<MySqlParameter> CreateParametersMySql(Category category,
            ActionType actionType)
        {
            var parameters = new List<MySqlParameter>();
            switch (actionType)
                {
                    case ActionType.Insert:
                        parameters.Add(new MySqlParameter()
                        {
                            ParameterName = "@Label",
                            MySqlDbType = MySqlDbType.VarChar,
                            Size = 100,
                            Value = category.Label ?? (object)DBNull.Value
                        });
                        parameters.Add(new MySqlParameter()
                        {
                            ParameterName = "@Description",
                            MySqlDbType = MySqlDbType.Text,
                            Value = category.Description ?? (object)DBNull.Value
                        });
                        break;
                    case ActionType.Update:
                        parameters.Add(new MySqlParameter()
                        {
                            ParameterName = "@Label",
                            MySqlDbType = MySqlDbType.VarChar,
                            Size = 100,
                            Value = category.Label ?? (object)DBNull.Value
                        });
                        parameters.Add(new MySqlParameter()
                        {
                            ParameterName = "@Description",
                            MySqlDbType = MySqlDbType.Text,
                            Value = category.Description ?? (object)DBNull.Value
                        });
                        parameters.Add(new MySqlParameter()
                        {
                            ParameterName = "@categoryId",
                            MySqlDbType = MySqlDbType.Int16,
                            Size = 50,
                            Value = category.IdCategory
                        });
                        break;
                    case ActionType.Delete:
                        parameters.Add(new MySqlParameter()
                        {
                            ParameterName = "@categoryId",
                            MySqlDbType = MySqlDbType.Int32,
                            Value = category.IdCategory
                        });
                        break;
            }
            
            return parameters;
        }
        #endregion
    }
}
