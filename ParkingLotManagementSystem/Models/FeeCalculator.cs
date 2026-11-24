namespace ParkingLotManagementSystem.Models;

public static class FeeCalculator
{
    public static int CalculateHours(DateTime entryTime, DateTime exitTime)
    {
        var totalHours = (exitTime - entryTime).TotalHours;
        if (totalHours <= 0)
        {
            return 1;
        }

        return Math.Max(1, (int)Math.Ceiling(totalHours));
    }

    public static decimal CalculateFee(int hours, decimal ratePerHour) => hours * ratePerHour;

    public static CheckOutQuote CreateQuote(DateTime entryTime, DateTime exitTime, decimal ratePerHour)
    {
        var hours = CalculateHours(entryTime, exitTime);
        return new CheckOutQuote
        {
            ExitTime = exitTime,
            Duration = exitTime - entryTime,
            Hours = hours,
            RatePerHour = ratePerHour,
            Fee = CalculateFee(hours, ratePerHour)
        };
    }
}
