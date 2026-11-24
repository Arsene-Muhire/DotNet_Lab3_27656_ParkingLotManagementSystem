namespace ParkingLotManagementSystem.Models;

public class ParkingRecord
{
    public int RecordID { get; set; }
    public string TicketNo { get; set; } = string.Empty;
    public int VehicleID { get; set; }
    public int SlotID { get; set; }
    public int UserID { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public int? Hours { get; set; }
    public decimal RatePerHour { get; set; }
    public decimal? Fee { get; set; }
    public decimal? AmountPaid { get; set; }
    public string? PaymentMethod { get; set; }
    public string Status { get; set; } = RecordStatuses.Parked;

    public string PlateNo { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string SlotNo { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string AttendantName { get; set; } = string.Empty;

    public bool IsParked => Status == RecordStatuses.Parked;
    public decimal? Change => AmountPaid.HasValue && Fee.HasValue ? AmountPaid.Value - Fee.Value : null;
    public TimeSpan Duration => (ExitTime ?? DateTime.Now) - EntryTime;
}
