using ParkingLotManagementSystem.Models;
using ParkingLotManagementSystem.Models.Data;

namespace ParkingLotManagementSystem.Controllers;

public class DashboardController : BaseController
{
    private readonly ReportRepository _reports = new();
    private readonly ParkingRecordRepository _records = new();

    public DashboardSummary GetSummary()
    {
        return _reports.GetDashboardSummary(DateTime.Today);
    }

    public List<ParkingRecord> GetCurrentlyParked()
    {
        return _records.GetCurrentlyParked();
    }
}
