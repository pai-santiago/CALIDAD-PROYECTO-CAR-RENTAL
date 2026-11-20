using MySqlConnector;

namespace CarRentalServices.Utilities;

public static class MySqlParameterBuilder
{
    public static MySqlParameter CreateParameter(
        string name,
        MySqlDbType type,
        int? size,
        object? value)
    {
        var parameter = new MySqlParameter
        {
            ParameterName = name,
            MySqlDbType = type,
            Value = value ?? DBNull.Value
        };

        if (size.HasValue)
            parameter.Size = size.Value;

        return parameter;
    }

    public static MySqlParameter CreateParameter(string name, MySqlDbType type, object? value)
        => CreateParameter(name, type, null, value);
}
