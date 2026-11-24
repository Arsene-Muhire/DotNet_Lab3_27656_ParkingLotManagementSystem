namespace ParkingLotManagementSystem.Models;

public class Rate
{
    public int RateID { get; set; }
    public string VehicleType { get; set; } = string.Empty;
    public decimal PricePerHour { get; set; }
}
