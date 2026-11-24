using MySql.Data.MySqlClient;
using ParkingLotManagementSystem.Models;
using ParkingLotManagementSystem.Models.Data;

namespace ParkingLotManagementSystem.Controllers;

public class SlotController : BaseController
{
    private readonly SlotRepository _slots = new();

    public List<ParkingSlot> GetSlots(string? status, string? vehicleType, string? zone, string? slotNo)
    {
        return _slots.Search(status, vehicleType, zone, slotNo);
    }

    public List<string> GetZones()
    {
        return _slots.GetZones();
    }

    public OperationResult Add(string slotNo, string vehicleType, string zone)
    {
        if (!IsAdministrator)
        {
            return OperationResult.Fail(AdministratorOnlyMessage);
        }

        var validation = Validate(slotNo, vehicleType, zone);
        if (validation != null)
        {
            return OperationResult.Fail(validation);
        }

        return Run(() =>
        {
            var cleanSlotNo = slotNo.Trim().ToUpperInvariant();
            if (_slots.SlotNoExists(cleanSlotNo, 0))
            {
                return OperationResult.Fail($"A slot with the number '{cleanSlotNo}' already exists.");
            }

            try
            {
                _slots.Insert(new ParkingSlot
                {
                    SlotNo = cleanSlotNo,
                    VehicleType = vehicleType,
                    Zone = zone.Trim(),
                    Status = SlotStatuses.Available
                });
            }
            catch (MySqlException ex) when (ex.IsUniqueViolation())
            {
                return OperationResult.Fail($"A slot with the number '{cleanSlotNo}' already exists.");
            }

            return OperationResult.Ok($"Slot {cleanSlotNo} has been added.");
        });
    }

    public OperationResult Update(int slotId, string slotNo, string vehicleType, string zone, string status)
    {
        if (!IsAdministrator)
        {
            return OperationResult.Fail(AdministratorOnlyMessage);
        }

        var validation = Validate(slotNo, vehicleType, zone);
        if (validation != null)
        {
            return OperationResult.Fail(validation);
        }

        return Run(() =>
        {
            var slot = _slots.GetById(slotId);
            if (slot == null)
            {
                return OperationResult.Fail("The selected slot no longer exists.");
            }

            if (slot.Status == SlotStatuses.Occupied)
            {
                if (vehicleType != slot.VehicleType || status != SlotStatuses.Occupied)
                {
                    return OperationResult.Fail("This slot is Occupied. Its vehicle type and status cannot be changed until the vehicle leaves.");
                }
            }
            else if (status != SlotStatuses.Available && status != SlotStatuses.OutOfService)
            {
                return OperationResult.Fail("A slot can only be set to Available or Out of Service manually.");
            }

            var cleanSlotNo = slotNo.Trim().ToUpperInvariant();
            if (_slots.SlotNoExists(cleanSlotNo, slotId))
            {
                return OperationResult.Fail($"A slot with the number '{cleanSlotNo}' already exists.");
            }

            slot.SlotNo = cleanSlotNo;
            slot.VehicleType = vehicleType;
            slot.Zone = zone.Trim();
            slot.Status = status;

            try
            {
                _slots.Update(slot);
            }
            catch (MySqlException ex) when (ex.IsUniqueViolation())
            {
                return OperationResult.Fail($"A slot with the number '{cleanSlotNo}' already exists.");
            }

            return OperationResult.Ok($"Slot {cleanSlotNo} has been updated.");
        });
    }

    public OperationResult ToggleOutOfService(int slotId)
    {
        if (!IsAdministrator)
        {
            return OperationResult.Fail(AdministratorOnlyMessage);
        }

        return Run(() =>
        {
            var slot = _slots.GetById(slotId);
            if (slot == null)
            {
                return OperationResult.Fail("The selected slot no longer exists.");
            }

            if (slot.Status == SlotStatuses.Occupied)
            {
                return OperationResult.Fail("An Occupied slot cannot be set Out of Service.");
            }

            slot.Status = slot.Status == SlotStatuses.OutOfService ? SlotStatuses.Available : SlotStatuses.OutOfService;
            _slots.Update(slot);
            return OperationResult.Ok($"Slot {slot.SlotNo} is now {slot.Status}.");
        });
    }

    public OperationResult Delete(int slotId)
    {
        if (!IsAdministrator)
        {
            return OperationResult.Fail(AdministratorOnlyMessage);
        }

        return Run(() =>
        {
            var slot = _slots.GetById(slotId);
            if (slot == null)
            {
                return OperationResult.Fail("The selected slot no longer exists.");
            }

            if (slot.Status == SlotStatuses.Occupied)
            {
                return OperationResult.Fail("An Occupied slot cannot be deleted.");
            }

            if (_slots.HasParkingRecords(slotId))
            {
                return OperationResult.Fail("This slot has parking records and cannot be deleted. You can set it Out of Service instead.");
            }

            try
            {
                _slots.Delete(slotId);
            }
            catch (MySqlException ex) when (ex.IsForeignKeyViolation())
            {
                return OperationResult.Fail("This slot has parking records and cannot be deleted.");
            }

            return OperationResult.Ok($"Slot {slot.SlotNo} has been deleted.");
        });
    }

    private static string? Validate(string slotNo, string vehicleType, string zone)
    {
        if (string.IsNullOrWhiteSpace(slotNo))
        {
            return "Please enter the slot number.";
        }

        if (slotNo.Trim().Length > 20)
        {
            return "The slot number must not be longer than 20 characters.";
        }

        if (!VehicleTypes.All.Contains(vehicleType))
        {
            return "Please select a valid vehicle type.";
        }

        if (string.IsNullOrWhiteSpace(zone))
        {
            return "Please enter the zone or floor.";
        }

        if (zone.Trim().Length > 50)
        {
            return "The zone must not be longer than 50 characters.";
        }

        return null;
    }
}
