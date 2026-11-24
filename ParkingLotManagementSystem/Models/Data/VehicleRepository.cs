using MySql.Data.MySqlClient;

namespace ParkingLotManagementSystem.Models.Data;

public class VehicleRepository
{
    private const string SelectColumns = "SELECT VehicleID, PlateNo, VehicleType, OwnerName, Phone FROM Vehicles";

    public Vehicle? GetByPlate(string plateNo)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection, $"{SelectColumns} WHERE PlateNo = @plate");
        command.AddParam("@plate", plateNo);
        return ReadSingle(command);
    }

    public Vehicle? GetByPlateForUpdate(MySqlConnection connection, MySqlTransaction transaction, string plateNo)
    {
        using var command = Database.CreateCommand(connection,
            $"{SelectColumns} WHERE PlateNo = @plate FOR UPDATE", transaction);
        command.AddParam("@plate", plateNo);
        return ReadSingle(command);
    }

    public int Insert(MySqlConnection connection, MySqlTransaction transaction, Vehicle vehicle)
    {
        using var command = Database.CreateCommand(connection,
            @"INSERT INTO Vehicles (PlateNo, VehicleType, OwnerName, Phone)
              VALUES (@plate, @type, @owner, @phone)", transaction);
        command.AddParam("@plate", vehicle.PlateNo)
               .AddParam("@type", vehicle.VehicleType)
               .AddParam("@owner", vehicle.OwnerName)
               .AddParam("@phone", string.IsNullOrWhiteSpace(vehicle.Phone) ? null : vehicle.Phone);
        return command.ExecuteInsert();
    }

    public void Update(MySqlConnection connection, MySqlTransaction transaction, Vehicle vehicle)
    {
        using var command = Database.CreateCommand(connection,
            @"UPDATE Vehicles
              SET VehicleType = @type, OwnerName = @owner, Phone = @phone
              WHERE VehicleID = @id", transaction);
        command.AddParam("@type", vehicle.VehicleType)
               .AddParam("@owner", vehicle.OwnerName)
               .AddParam("@phone", string.IsNullOrWhiteSpace(vehicle.Phone) ? null : vehicle.Phone)
               .AddParam("@id", vehicle.VehicleID);
        command.ExecuteNonQuery();
    }

    private static Vehicle? ReadSingle(MySqlCommand command)
    {
        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new Vehicle
        {
            VehicleID = reader.GetInt("VehicleID"),
            PlateNo = reader.GetText("PlateNo"),
            VehicleType = reader.GetText("VehicleType"),
            OwnerName = reader.GetText("OwnerName"),
            Phone = reader.GetText("Phone")
        };
    }
}
