namespace ParkingLotManagementSystem.Models;

public class DashboardSummary
{
    public int TotalSlots { get; set; }
    public int AvailableSlots { get; set; }
    public int OccupiedSlots { get; set; }
    public int OutOfServiceSlots { get; set; }
    public int VehiclesParked { get; set; }
    public decimal IncomeToday { get; set; }
}

public class IncomeGroupRow
{
    public string GroupName { get; set; } = string.Empty;
    public int Vehicles { get; set; }
    public decimal TotalIncome { get; set; }
}

public class SlotOccupancyRow
{
    public string VehicleType { get; set; } = string.Empty;
    public int TotalSlots { get; set; }
    public int Available { get; set; }
    public int Occupied { get; set; }
    public int OutOfService { get; set; }
}

public class DailyIncomeReport
{
    public DateTime Date { get; set; }
    public List<ParkingRecord> Records { get; set; } = new();
    public int VehicleCount => Records.Count;
    public decimal TotalIncome => Records.Sum(r => r.Fee ?? 0m);
}

public class PeriodIncomeReport
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public List<IncomeGroupRow> ByVehicleType { get; set; } = new();
    public List<IncomeGroupRow> ByPaymentMethod { get; set; } = new();
    public int TotalVehicles => ByVehicleType.Sum(r => r.Vehicles);
    public decimal TotalIncome => ByVehicleType.Sum(r => r.TotalIncome);
}

public class CheckOutQuote
{
    public DateTime ExitTime { get; set; }
    public TimeSpan Duration { get; set; }
    public int Hours { get; set; }
    public decimal RatePerHour { get; set; }
    public decimal Fee { get; set; }
}
