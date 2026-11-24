using MySql.Data.MySqlClient;
using ParkingLotManagementSystem.Models;

namespace ParkingLotManagementSystem.Controllers;

public abstract class BaseController
{
    protected const string AdministratorOnlyMessage = "Only the Administrator can perform this action.";

    protected static User CurrentUser =>
        Session.CurrentUser ?? throw new InvalidOperationException("No user is logged in.");

    protected static bool IsAdministrator => Session.IsAdministrator;

    protected static OperationResult Run(Func<OperationResult> action)
    {
        try
        {
            return action();
        }
        catch (BusinessRuleException ex)
        {
            return OperationResult.Fail(ex.Message);
        }
        catch (MySqlException ex)
        {
            return OperationResult.Fail("Database error: " + ex.Message);
        }
    }

    protected static OperationResult<T> Run<T>(Func<OperationResult<T>> action)
    {
        try
        {
            return action();
        }
        catch (BusinessRuleException ex)
        {
            return OperationResult<T>.Fail(ex.Message);
        }
        catch (MySqlException ex)
        {
            return OperationResult<T>.Fail("Database error: " + ex.Message);
        }
    }
}
