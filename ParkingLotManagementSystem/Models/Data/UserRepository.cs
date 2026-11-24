using MySql.Data.MySqlClient;

namespace ParkingLotManagementSystem.Models.Data;

public class UserRepository
{
    private const string SelectColumns =
        "SELECT UserID, FullName, Username, PasswordHash, Role, IsActive FROM Users";

    public User? GetByUsername(string username)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection, $"{SelectColumns} WHERE Username = @username");
        command.AddParam("@username", username);
        return ReadSingle(command);
    }

    public User? GetById(int userId)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection, $"{SelectColumns} WHERE UserID = @id");
        command.AddParam("@id", userId);
        return ReadSingle(command);
    }

    public List<User> Search(string keyword)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            $@"{SelectColumns}
               WHERE @keyword IS NULL OR FullName LIKE @keyword OR Username LIKE @keyword OR Role LIKE @keyword
               ORDER BY FullName");
        command.AddParam("@keyword", string.IsNullOrWhiteSpace(keyword) ? null : SqlExtensions.ToLikePattern(keyword.Trim()));
        return ReadList(command);
    }

    public bool UsernameExists(string username, int excludeUserId)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            "SELECT COUNT(*) FROM Users WHERE Username = @username AND UserID <> @id");
        command.AddParam("@username", username).AddParam("@id", excludeUserId);
        return command.ExecuteCount() > 0;
    }

    public int CountActiveAdministrators(int excludeUserId)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            "SELECT COUNT(*) FROM Users WHERE Role = @role AND IsActive = 1 AND UserID <> @id");
        command.AddParam("@role", Roles.Administrator).AddParam("@id", excludeUserId);
        return command.ExecuteCount();
    }

    public bool HasParkingRecords(int userId)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            "SELECT COUNT(*) FROM ParkingRecords WHERE UserID = @id");
        command.AddParam("@id", userId);
        return command.ExecuteCount() > 0;
    }

    public int Insert(User user)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            @"INSERT INTO Users (FullName, Username, PasswordHash, Role, IsActive)
              VALUES (@name, @username, @hash, @role, @active)");
        command.AddParam("@name", user.FullName)
               .AddParam("@username", user.Username)
               .AddParam("@hash", user.PasswordHash)
               .AddParam("@role", user.Role)
               .AddParam("@active", user.IsActive);
        return command.ExecuteInsert();
    }

    public void Update(User user)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            @"UPDATE Users
              SET FullName = @name, Username = @username, Role = @role, IsActive = @active
              WHERE UserID = @id");
        command.AddParam("@name", user.FullName)
               .AddParam("@username", user.Username)
               .AddParam("@role", user.Role)
               .AddParam("@active", user.IsActive)
               .AddParam("@id", user.UserID);
        command.ExecuteNonQuery();
    }

    public void UpdatePassword(int userId, string passwordHash)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            "UPDATE Users SET PasswordHash = @hash WHERE UserID = @id");
        command.AddParam("@hash", passwordHash).AddParam("@id", userId);
        command.ExecuteNonQuery();
    }

    public void SetActive(int userId, bool isActive)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            "UPDATE Users SET IsActive = @active WHERE UserID = @id");
        command.AddParam("@active", isActive).AddParam("@id", userId);
        command.ExecuteNonQuery();
    }

    public void Delete(int userId)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection, "DELETE FROM Users WHERE UserID = @id");
        command.AddParam("@id", userId);
        command.ExecuteNonQuery();
    }

    private static User? ReadSingle(MySqlCommand command)
    {
        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    private static List<User> ReadList(MySqlCommand command)
    {
        var users = new List<User>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            users.Add(Map(reader));
        }

        return users;
    }

    private static User Map(MySqlDataReader reader)
    {
        return new User
        {
            UserID = reader.GetInt("UserID"),
            FullName = reader.GetText("FullName"),
            Username = reader.GetText("Username"),
            PasswordHash = reader.GetText("PasswordHash"),
            Role = reader.GetText("Role"),
            IsActive = reader.GetFlag("IsActive")
        };
    }
}
