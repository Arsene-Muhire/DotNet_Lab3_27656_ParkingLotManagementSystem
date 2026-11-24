namespace ParkingLotManagementSystem.Models;

public static class Roles
{
    public const string Administrator = "Administrator";
    public const string Attendant = "Attendant";
    public static readonly string[] All = { Administrator, Attendant };
}

public static class VehicleTypes
{
    public const string Motorcycle = "Motorcycle";
    public const string Car = "Car";
    public const string BusTruck = "Bus/Truck";
    public static readonly string[] All = { Motorcycle, Car, BusTruck };
}

public static class SlotStatuses
{
    public const string Available = "Available";
    public const string Occupied = "Occupied";
    public const string OutOfService = "Out of Service";
    public static readonly string[] All = { Available, Occupied, OutOfService };
}

public static class RecordStatuses
{
    public const string Parked = "Parked";
    public const string Completed = "Completed";
    public static readonly string[] All = { Parked, Completed };
}

public static class PaymentMethods
{
    public const string Cash = "Cash";
    public const string MobileMoney = "Mobile Money";
    public static readonly string[] All = { Cash, MobileMoney };
}
