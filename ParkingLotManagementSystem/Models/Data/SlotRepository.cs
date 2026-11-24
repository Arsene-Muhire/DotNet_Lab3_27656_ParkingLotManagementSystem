using MySql.Data.MySqlClient;

namespace ParkingLotManagementSystem.Models.Data;

public class SlotRepository
{
    private const string SelectColumns = "SELECT SlotID, SlotNo, VehicleType, Zone, Status FROM ParkingSlots";

    public List<ParkingSlot> Search(string? status, string? vehicleType, string? zone, string? slotNo)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            $@"{SelectColumns}
               WHERE (@status IS NULL OR Status = @status)
                 AND (@type IS NULL OR VehicleType = @type)
                 AND (@zone IS NULL OR Zone = @zone)
                 AND (@slotNo IS NULL OR SlotNo LIKE @slotNo)
               ORDER BY SlotNo");
        command.AddParam("@status", status)
               .AddParam("@type", vehicleType)
               .AddParam("@zone", zone)
               .AddParam("@slotNo", string.IsNullOrWhiteSpace(slotNo) ? null : SqlExtensions.ToLikePattern(slotNo.Trim()));
        return ReadList(command);
    }

    public List<ParkingSlot> GetAvailableByType(string vehicleType)
    {
        return Search(SlotStatuses.Available, vehicleType, null, null);
    }

    public ParkingSlot? GetById(int slotId)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection, $"{SelectColumns} WHERE SlotID = @id");
        command.AddParam("@id", slotId);
        var slots = ReadList(command);
        return slots.Count > 0 ? slots[0] : null;
    }

    public List<string> GetZones()
    {
        var zones = new List<string>();
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection, "SELECT DISTINCT Zone FROM ParkingSlots ORDER BY Zone");
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            zones.Add(reader.GetString(0));
        }

        return zones;
    }

    public bool SlotNoExists(string slotNo, int excludeSlotId)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            "SELECT COUNT(*) FROM ParkingSlots WHERE SlotNo = @slotNo AND SlotID <> @id");
        command.AddParam("@slotNo", slotNo).AddParam("@id", excludeSlotId);
        return command.ExecuteCount() > 0;
    }

    public bool HasParkingRecords(int slotId)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            "SELECT COUNT(*) FROM ParkingRecords WHERE SlotID = @id");
        command.AddParam("@id", slotId);
        return command.ExecuteCount() > 0;
    }

    public int Insert(ParkingSlot slot)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            @"INSERT INTO ParkingSlots (SlotNo, VehicleType, Zone, Status)
              VALUES (@slotNo, @type, @zone, @status)");
        command.AddParam("@slotNo", slot.SlotNo)
               .AddParam("@type", slot.VehicleType)
               .AddParam("@zone", slot.Zone)
               .AddParam("@status", slot.Status);
        return command.ExecuteInsert();
    }

    public void Update(ParkingSlot slot)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            @"UPDATE ParkingSlots
              SET SlotNo = @slotNo, VehicleType = @type, Zone = @zone, Status = @status
              WHERE SlotID = @id");
        command.AddParam("@slotNo", slot.SlotNo)
               .AddParam("@type", slot.VehicleType)
               .AddParam("@zone", slot.Zone)
               .AddParam("@status", slot.Status)
               .AddParam("@id", slot.SlotID);
        command.ExecuteNonQuery();
    }

    public void Delete(int slotId)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection, "DELETE FROM ParkingSlots WHERE SlotID = @id");
        command.AddParam("@id", slotId);
        command.ExecuteNonQuery();
    }

    private static List<ParkingSlot> ReadList(MySqlCommand command)
    {
        var slots = new List<ParkingSlot>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            slots.Add(new ParkingSlot
            {
                SlotID = reader.GetInt("SlotID"),
                SlotNo = reader.GetText("SlotNo"),
                VehicleType = reader.GetText("VehicleType"),
                Zone = reader.GetText("Zone"),
                Status = reader.GetText("Status")
            });
        }

        return slots;
    }
}
