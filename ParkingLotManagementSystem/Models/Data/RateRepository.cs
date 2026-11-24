using MySql.Data.MySqlClient;

namespace ParkingLotManagementSystem.Models.Data;

public class RateRepository
{
    private const string SelectColumns = "SELECT RateID, VehicleType, PricePerHour FROM Rates";

    public List<Rate> GetAll()
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            $@"{SelectColumns}
               ORDER BY CASE VehicleType WHEN 'Motorcycle' THEN 1 WHEN 'Car' THEN 2 ELSE 3 END");
        return ReadList(command);
    }

    public Rate? GetByVehicleType(string vehicleType)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection, $"{SelectColumns} WHERE VehicleType = @type");
        command.AddParam("@type", vehicleType);
        var rates = ReadList(command);
        return rates.Count > 0 ? rates[0] : null;
    }

    public void Save(string vehicleType, decimal pricePerHour)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            @"INSERT INTO Rates (VehicleType, PricePerHour) VALUES (@type, @price)
              ON DUPLICATE KEY UPDATE PricePerHour = VALUES(PricePerHour)");
        command.AddParam("@type", vehicleType).AddParam("@price", pricePerHour);
        command.ExecuteNonQuery();
    }

    private static List<Rate> ReadList(MySqlCommand command)
    {
        var rates = new List<Rate>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rates.Add(new Rate
            {
                RateID = reader.GetInt("RateID"),
                VehicleType = reader.GetText("VehicleType"),
                PricePerHour = reader.GetMoney("PricePerHour")
            });
        }

        return rates;
    }
}
