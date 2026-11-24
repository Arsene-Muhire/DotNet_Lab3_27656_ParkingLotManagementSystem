using ParkingLotManagementSystem.Models;
using ParkingLotManagementSystem.Models.Data;

namespace ParkingLotManagementSystem.Controllers;

public class RateController : BaseController
{
    private readonly RateRepository _rates = new();

    public bool CanEditRates => IsAdministrator;

    public List<Rate> GetRates()
    {
        return _rates.GetAll();
    }

    public OperationResult SaveRate(string vehicleType, decimal pricePerHour)
    {
        if (!IsAdministrator)
        {
            return OperationResult.Fail(AdministratorOnlyMessage);
        }

        if (!VehicleTypes.All.Contains(vehicleType))
        {
            return OperationResult.Fail("Please select a valid vehicle type.");
        }

        if (pricePerHour <= 0)
        {
            return OperationResult.Fail("The price per hour must be greater than zero.");
        }

        return Run(() =>
        {
            _rates.Save(vehicleType, pricePerHour);
            return OperationResult.Ok($"The rate for {vehicleType} is now {pricePerHour:#,##0.##} RWF per hour. It applies to vehicles checked in from now on.");
        });
    }
}
