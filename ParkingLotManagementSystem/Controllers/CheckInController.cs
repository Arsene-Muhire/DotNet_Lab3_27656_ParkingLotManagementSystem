using ParkingLotManagementSystem.Models;
using ParkingLotManagementSystem.Models.Data;

namespace ParkingLotManagementSystem.Controllers;

public class CheckInController : BaseController
{
    private readonly SlotRepository _slots = new();
    private readonly RateRepository _rates = new();
    private readonly VehicleRepository _vehicles = new();
    private readonly ParkingRecordRepository _records = new();

    public List<ParkingSlot> GetAvailableSlots(string vehicleType)
    {
        return VehicleTypes.All.Contains(vehicleType) ? _slots.GetAvailableByType(vehicleType) : new List<ParkingSlot>();
    }

    public Rate? GetRate(string vehicleType)
    {
        return _rates.GetByVehicleType(vehicleType);
    }

    public Vehicle? FindVehicle(string plateNo)
    {
        var plate = Vehicle.NormalizePlate(plateNo);
        return plate.Length == 0 ? null : _vehicles.GetByPlate(plate);
    }

    public OperationResult<ParkingRecord> CheckIn(string plateNo, string vehicleType, string ownerName, string phone, int? slotId)
    {
        var plate = Vehicle.NormalizePlate(plateNo);
        if (plate.Length == 0)
        {
            return OperationResult<ParkingRecord>.Fail("Please enter the plate number.");
        }

        if (plate.Length > 20)
        {
            return OperationResult<ParkingRecord>.Fail("The plate number must not be longer than 20 characters.");
        }

        if (!VehicleTypes.All.Contains(vehicleType))
        {
            return OperationResult<ParkingRecord>.Fail("Please select the vehicle type.");
        }

        if (string.IsNullOrWhiteSpace(ownerName))
        {
            return OperationResult<ParkingRecord>.Fail("Please enter the driver or owner name.");
        }

        var cleanPhone = phone?.Trim() ?? string.Empty;
        if (cleanPhone.Length > 0 && (cleanPhone.Length > 20 || !cleanPhone.All(c => char.IsDigit(c) || c == '+' || c == ' ' || c == '-')))
        {
            return OperationResult<ParkingRecord>.Fail("The phone number may only contain digits, spaces, '+' and '-'.");
        }

        return Run(() =>
        {
            if (_records.IsVehicleParked(plate))
            {
                return OperationResult<ParkingRecord>.Fail($"Vehicle {plate} is already parked in the lot. It must be checked out first.");
            }

            var availableSlots = _slots.GetAvailableByType(vehicleType);
            if (availableSlots.Count == 0)
            {
                return OperationResult<ParkingRecord>.Fail($"No slot is available for {vehicleType} vehicles at the moment.");
            }

            if (slotId == null)
            {
                return OperationResult<ParkingRecord>.Fail("Please choose an available slot.");
            }

            if (availableSlots.All(s => s.SlotID != slotId.Value))
            {
                return OperationResult<ParkingRecord>.Fail("The selected slot is not available for this vehicle type. Please choose another slot.");
            }

            var rate = _rates.GetByVehicleType(vehicleType);
            if (rate == null)
            {
                return OperationResult<ParkingRecord>.Fail($"No parking rate is set for {vehicleType}. Please ask the Administrator to set it.");
            }

            var vehicle = new Vehicle
            {
                PlateNo = plate,
                VehicleType = vehicleType,
                OwnerName = ownerName.Trim(),
                Phone = cleanPhone
            };

            var record = _records.CheckIn(vehicle, slotId.Value, CurrentUser.UserID, rate.PricePerHour, DateTime.Now);
            return OperationResult<ParkingRecord>.Ok(record, $"Vehicle {plate} has been checked in to slot {record.SlotNo}.");
        });
    }
}
