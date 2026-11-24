using ParkingLotManagementSystem.Models;
using ParkingLotManagementSystem.Models.Data;

namespace ParkingLotManagementSystem.Controllers;

public class RecordController : BaseController
{
    private readonly ParkingRecordRepository _records = new();

    public bool CanDeleteRecords => IsAdministrator;

    public List<ParkingRecord> Search(string? keyword, DateTime? entryDate, string? status, string? vehicleType)
    {
        return _records.Search(keyword, entryDate, status, vehicleType);
    }

    public ParkingRecord? GetById(int recordId)
    {
        return _records.GetById(recordId);
    }

    public OperationResult Delete(int recordId)
    {
        if (!IsAdministrator)
        {
            return OperationResult.Fail(AdministratorOnlyMessage);
        }

        return Run(() =>
        {
            var record = _records.GetById(recordId);
            if (record == null)
            {
                return OperationResult.Fail("The selected record no longer exists.");
            }

            if (!record.IsParked)
            {
                return OperationResult.Fail("A completed record cannot be deleted.");
            }

            _records.DeleteParked(record.RecordID, record.SlotID);
            return OperationResult.Ok($"Record {record.TicketNo} has been deleted and slot {record.SlotNo} is Available again.");
        });
    }
}
