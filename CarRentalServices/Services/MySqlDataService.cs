using CarRentalServices.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System.Data;

namespace CarRentalServices.Services
{
    public class MySqlDataService(IConfiguration manager, ILogger<MySqlDataService> logger) : IMySqlDataService
    {
        readonly IConfiguration _configuration = manager;
        readonly ILogger<MySqlDataService> _logger = logger;
        private const string _connStringName = "MysqlConnection";
        public bool BulkInsert(DataTable dtData, string tableName)
        {
            throw new NotImplementedException();
        }

        public DataSet ExecSP(string nameSp)
        {
            return ExecSP(nameSp, []);
        }

        public DataSet ExecSP(string nameSp, IEnumerable<MySqlParameter> parameters)
        {
            var dsData = new DataSet();
            try
            {
                using var conn = new MySqlConnection(_configuration.GetConnectionString(_connStringName));
                var sqlCom = new MySqlCommand(nameSp, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                if (parameters.Any())
                {
                    sqlCom.Parameters.AddRange(parameters.ToArray());
                }
                var daDatos = new MySqlDataAdapter(sqlCom);
                daDatos.Fill(dsData);
            }
            catch (MySqlException sqle)
            {
                _logger.LogError(sqle, "Error executing MySQL stored procedure {StoredProcedure}.", nameSp);
                throw;
            }
            return dsData;
        }

        public DataSet ExecuteSql(string sqlText)
        {
            return ExecuteSql(sqlText, null);
        }

        public DataSet ExecuteSql(string sqlText, IEnumerable<MySqlParameter>? parameters)
        {
            var dsData = new DataSet();
            try
            {
                using var conn = new MySqlConnection(_configuration.GetConnectionString(_connStringName));
                using var sqlCom = new MySqlCommand(sqlText, conn)
                {
                    CommandType = CommandType.Text
                };
                if (parameters is not null)
                {
                    sqlCom.Parameters.AddRange(parameters.ToArray());
                }
                conn.Open();
                var daDatos = new MySqlDataAdapter(sqlCom);
                daDatos.Fill(dsData);
            }
            catch (MySqlException sqle)
            {
                _logger.LogError(sqle, "Error executing MySQL query.");
                throw;
            }

            return dsData;
        }

        public int ExecuteNonQuery(string sqlText, IEnumerable<MySqlParameter>? parameters = null)
        {
            try
            {
                using var conn = new MySqlConnection(_configuration.GetConnectionString(_connStringName));
                using var command = new MySqlCommand(sqlText, conn)
                {
                    CommandType = CommandType.Text
                };
                if (parameters is not null)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }

                conn.Open();
                return command.ExecuteNonQuery();
            }
            catch (MySqlException exception)
            {
                _logger.LogError(exception, "Error executing MySQL command.");
                throw;
            }
        }

        public long ExecuteInsert(string sqlText, IEnumerable<MySqlParameter> parameters)
        {
            try
            {
                using var conn = new MySqlConnection(_configuration.GetConnectionString(_connStringName));
                using var command = new MySqlCommand(sqlText, conn)
                {
                    CommandType = CommandType.Text
                };
                if (parameters.Any())
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }

                conn.Open();
                command.ExecuteNonQuery();
                return command.LastInsertedId;
            }
            catch (MySqlException exception)
            {
                _logger.LogError(exception, "Error executing MySQL insert.");
                throw;
            }
        }
    }
}
