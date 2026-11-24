using ParkingLotManagementSystem.Models;
using ParkingLotManagementSystem.Models.Data;

namespace ParkingLotManagementSystem.Controllers;

public class CheckOutController : BaseController
{
    private readonly ParkingRecordRepository _records = new();

    public OperationResult<ParkingRecord> FindParkedVehicle(string ticketOrPlate)
    {
        if (string.IsNullOrWhiteSpace(ticketOrPlate))
        {
            return OperationResult<ParkingRecord>.Fail("Please enter the ticket number or the plate number.");
        }

        return Run(() =>
        {
            var ticket = ticketOrPlate.Trim().ToUpperInvariant();
            var plate = Vehicle.NormalizePlate(ticketOrPlate);
            var record = _records.FindParked(ticket, plate);
            return record == null
                ? OperationResult<ParkingRecord>.Fail($"No parked vehicle was found with the ticket or plate number '{ticketOrPlate.Trim()}'.")
                : OperationResult<ParkingRecord>.Ok(record);
        });
    }

    public CheckOutQuote CalculateQuote(ParkingRecord record, DateTime exitTime)
    {
        return FeeCalculator.CreateQuote(record.EntryTime, exitTime, record.RatePerHour);
    }

    public OperationResult<ParkingRecord> CompleteCheckOut(int recordId, decimal amountReceived, string paymentMethod)
    {
        if (!PaymentMethods.All.Contains(paymentMethod))
        {
            return OperationResult<ParkingRecord>.Fail("Please choose the payment method (Cash or Mobile Money).");
        }

        if (amountReceived <= 0)
        {
            return OperationResult<ParkingRecord>.Fail("Please enter the amount received.");
        }

        return Run(() =>
        {
            var record = _records.GetById(recordId);
            if (record == null || !record.IsParked)
            {
                return OperationResult<ParkingRecord>.Fail("This vehicle is no longer parked. It may have been checked out already.");
            }

            var quote = CalculateQuote(record, DateTime.Now);
            if (amountReceived < quote.Fee)
            {
                return OperationResult<ParkingRecord>.Fail(
                    $"The amount received ({amountReceived:#,##0.##} RWF) is less than the fee ({quote.Fee:#,##0.##} RWF). The check-out cannot be completed.");
            }

            _records.CheckOut(record.RecordID, record.SlotID, quote.ExitTime, quote.Hours, quote.Fee, amountReceived, paymentMethod);
            var completed = _records.GetById(record.RecordID)!;
            return OperationResult<ParkingRecord>.Ok(completed, $"Vehicle {completed.PlateNo} has been checked out. Slot {completed.SlotNo} is now Available.");
        });
    }
}
