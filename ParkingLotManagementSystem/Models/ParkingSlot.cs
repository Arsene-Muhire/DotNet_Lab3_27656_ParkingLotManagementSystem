namespace ParkingLotManagementSystem.Models;

public class ParkingSlot
{
    public int SlotID { get; set; }
    public string SlotNo { get; set; } = string.Empty;
    public string VehicleType { get; set; } = VehicleTypes.Car;
    public string Zone { get; set; } = string.Empty;
    public string Status { get; set; } = SlotStatuses.Available;

    public override string ToString() => $"{SlotNo}   -   {Zone}";
}
