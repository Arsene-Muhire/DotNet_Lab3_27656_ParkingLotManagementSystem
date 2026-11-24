namespace ParkingLotManagementSystem.Models.Data;

public class ReportRepository
{
    public DashboardSummary GetDashboardSummary(DateTime today)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            @"SELECT
                  (SELECT COUNT(*) FROM ParkingSlots) AS TotalSlots,
                  (SELECT COUNT(*) FROM ParkingSlots WHERE Status = 'Available') AS AvailableSlots,
                  (SELECT COUNT(*) FROM ParkingSlots WHERE Status = 'Occupied') AS OccupiedSlots,
                  (SELECT COUNT(*) FROM ParkingSlots WHERE Status = 'Out of Service') AS OutOfServiceSlots,
                  (SELECT COUNT(*) FROM ParkingRecords WHERE Status = 'Parked') AS VehiclesParked,
                  (SELECT IFNULL(SUM(Fee), 0) FROM ParkingRecords
                   WHERE Status = 'Completed' AND ExitTime >= @from AND ExitTime < @to) AS IncomeToday");
        command.AddParam("@from", today.Date).AddParam("@to", today.Date.AddDays(1));

        using var reader = command.ExecuteReader();
        reader.Read();
        return new DashboardSummary
        {
            TotalSlots = reader.GetInt("TotalSlots"),
            AvailableSlots = reader.GetInt("AvailableSlots"),
            OccupiedSlots = reader.GetInt("OccupiedSlots"),
            OutOfServiceSlots = reader.GetInt("OutOfServiceSlots"),
            VehiclesParked = reader.GetInt("VehiclesParked"),
            IncomeToday = reader.GetMoney("IncomeToday")
        };
    }

    public List<IncomeGroupRow> GetIncomeByVehicleType(DateTime from, DateTime toExclusive)
    {
        return GetIncomeGrouped("v.VehicleType", from, toExclusive);
    }

    public List<IncomeGroupRow> GetIncomeByPaymentMethod(DateTime from, DateTime toExclusive)
    {
        return GetIncomeGrouped("r.PaymentMethod", from, toExclusive);
    }

    public List<SlotOccupancyRow> GetSlotOccupancy()
    {
        var rows = new List<SlotOccupancyRow>();
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            @"SELECT VehicleType,
                     COUNT(*) AS TotalSlots,
                     SUM(CASE WHEN Status = 'Available' THEN 1 ELSE 0 END) AS Available,
                     SUM(CASE WHEN Status = 'Occupied' THEN 1 ELSE 0 END) AS Occupied,
                     SUM(CASE WHEN Status = 'Out of Service' THEN 1 ELSE 0 END) AS OutOfService
              FROM ParkingSlots
              GROUP BY VehicleType
              ORDER BY CASE VehicleType WHEN 'Motorcycle' THEN 1 WHEN 'Car' THEN 2 ELSE 3 END");
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new SlotOccupancyRow
            {
                VehicleType = reader.GetText("VehicleType"),
                TotalSlots = reader.GetInt("TotalSlots"),
                Available = reader.GetInt("Available"),
                Occupied = reader.GetInt("Occupied"),
                OutOfService = reader.GetInt("OutOfService")
            });
        }

        return rows;
    }

    private static List<IncomeGroupRow> GetIncomeGrouped(string groupColumn, DateTime from, DateTime toExclusive)
    {
        var rows = new List<IncomeGroupRow>();
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            $@"SELECT {groupColumn} AS GroupName, COUNT(*) AS Vehicles, IFNULL(SUM(r.Fee), 0) AS TotalIncome
               FROM ParkingRecords r
               INNER JOIN Vehicles v ON v.VehicleID = r.VehicleID
               WHERE r.Status = 'Completed' AND r.ExitTime >= @from AND r.ExitTime < @to
               GROUP BY {groupColumn}
               ORDER BY TotalIncome DESC");
        command.AddParam("@from", from).AddParam("@to", toExclusive);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new IncomeGroupRow
            {
                GroupName = reader.GetText("GroupName"),
                Vehicles = reader.GetInt("Vehicles"),
                TotalIncome = reader.GetMoney("TotalIncome")
            });
        }

        return rows;
    }
}
