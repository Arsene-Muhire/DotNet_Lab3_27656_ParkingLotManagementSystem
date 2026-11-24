using ParkingLotManagementSystem.Models;
using ParkingLotManagementSystem.Models.Data;

namespace ParkingLotManagementSystem.Controllers;

public class ReportController : BaseController
{
    private readonly ReportRepository _reports = new();
    private readonly ParkingRecordRepository _records = new();

    public bool CanViewAllReports => IsAdministrator;

    public List<ParkingRecord> GetCurrentlyParked()
    {
        return _records.GetCurrentlyParked();
    }

    public DailyIncomeReport GetDailyIncome(DateTime date)
    {
        return new DailyIncomeReport
        {
            Date = date.Date,
            Records = _records.GetCompletedBetween(date.Date, date.Date.AddDays(1))
        };
    }

    public OperationResult<PeriodIncomeReport> GetPeriodIncome(DateTime from, DateTime to)
    {
        if (!IsAdministrator)
        {
            return OperationResult<PeriodIncomeReport>.Fail(AdministratorOnlyMessage);
        }

        if (to.Date < from.Date)
        {
            return OperationResult<PeriodIncomeReport>.Fail("The end date must be the same as or after the start date.");
        }

        return Run(() =>
        {
            var start = from.Date;
            var end = to.Date.AddDays(1);
            return OperationResult<PeriodIncomeReport>.Ok(new PeriodIncomeReport
            {
                From = start,
                To = to.Date,
                ByVehicleType = _reports.GetIncomeByVehicleType(start, end),
                ByPaymentMethod = _reports.GetIncomeByPaymentMethod(start, end)
            });
        });
    }

    public OperationResult<List<SlotOccupancyRow>> GetSlotOccupancy()
    {
        if (!IsAdministrator)
        {
            return OperationResult<List<SlotOccupancyRow>>.Fail(AdministratorOnlyMessage);
        }

        return Run(() => OperationResult<List<SlotOccupancyRow>>.Ok(_reports.GetSlotOccupancy()));
    }
}
