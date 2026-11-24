namespace ParkingLotManagementSystem.Models;

public class Vehicle
{
    public int VehicleID { get; set; }
    public string PlateNo { get; set; } = string.Empty;
    public string VehicleType { get; set; } = VehicleTypes.Car;
    public string OwnerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public static string NormalizePlate(string? plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
        {
            return string.Empty;
        }

        return new string(plate.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToUpperInvariant();
    }
}
