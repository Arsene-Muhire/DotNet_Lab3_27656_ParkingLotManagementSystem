using System.Configuration;
using MySql.Data.MySqlClient;

namespace ParkingLotManagementSystem.Models.Data;

public static class Database
{
    private const string DefaultConnectionString =
        "Server=localhost;Port=3306;Database=plms_db;Uid=root;Pwd=;SslMode=Disabled;AllowPublicKeyRetrieval=True;CharSet=utf8mb4;";

    public static string ConnectionString
    {
        get
        {
            var setting = ConfigurationManager.ConnectionStrings["PLMS"]?.ConnectionString;
            return string.IsNullOrWhiteSpace(setting) ? DefaultConnectionString : setting;
        }
    }

    public static MySqlConnection OpenConnection()
    {
        var connection = new MySqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    public static MySqlCommand CreateCommand(MySqlConnection connection, string sql, MySqlTransaction? transaction = null)
    {
        return new MySqlCommand(sql, connection, transaction);
    }
}

public static class SqlExtensions
{
    public static MySqlCommand AddParam(this MySqlCommand command, string name, object? value)
    {
        command.Parameters.AddWithValue(name, value ?? DBNull.Value);
        return command;
    }

    public static int ExecuteCount(this MySqlCommand command)
    {
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public static int ExecuteInsert(this MySqlCommand command)
    {
        command.ExecuteNonQuery();
        return Convert.ToInt32(command.LastInsertedId);
    }

    public static string ToLikePattern(string text)
    {
        var escaped = text.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
        return $"%{escaped}%";
    }

    public static string GetText(this MySqlDataReader reader, string column)
    {
        var index = reader.GetOrdinal(column);
        return reader.IsDBNull(index) ? string.Empty : reader.GetString(index);
    }

    public static string? GetNullableText(this MySqlDataReader reader, string column)
    {
        var index = reader.GetOrdinal(column);
        return reader.IsDBNull(index) ? null : reader.GetString(index);
    }

    public static int GetInt(this MySqlDataReader reader, string column)
    {
        var index = reader.GetOrdinal(column);
        return reader.IsDBNull(index) ? 0 : Convert.ToInt32(reader.GetValue(index));
    }

    public static int? GetNullableInt(this MySqlDataReader reader, string column)
    {
        var index = reader.GetOrdinal(column);
        return reader.IsDBNull(index) ? null : Convert.ToInt32(reader.GetValue(index));
    }

    public static bool GetFlag(this MySqlDataReader reader, string column)
    {
        var index = reader.GetOrdinal(column);
        return !reader.IsDBNull(index) && Convert.ToBoolean(reader.GetValue(index));
    }

    public static decimal GetMoney(this MySqlDataReader reader, string column)
    {
        var index = reader.GetOrdinal(column);
        return reader.IsDBNull(index) ? 0m : Convert.ToDecimal(reader.GetValue(index));
    }

    public static decimal? GetNullableMoney(this MySqlDataReader reader, string column)
    {
        var index = reader.GetOrdinal(column);
        return reader.IsDBNull(index) ? null : Convert.ToDecimal(reader.GetValue(index));
    }

    public static DateTime GetDate(this MySqlDataReader reader, string column)
    {
        return reader.GetDateTime(reader.GetOrdinal(column));
    }

    public static DateTime? GetNullableDate(this MySqlDataReader reader, string column)
    {
        var index = reader.GetOrdinal(column);
        return reader.IsDBNull(index) ? null : reader.GetDateTime(index);
    }

    public static bool IsUniqueViolation(this MySqlException exception)
    {
        return exception.Number == 1062;
    }

    public static bool IsForeignKeyViolation(this MySqlException exception)
    {
        return exception.Number == 1451 || exception.Number == 1452;
    }
}
