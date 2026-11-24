using ParkingLotManagementSystem.Helpers;
using ParkingLotManagementSystem.Models;
using ParkingLotManagementSystem.Models.Data;

namespace ParkingLotManagementSystem.Controllers;

public class AuthController : BaseController
{
    private readonly UserRepository _users = new();

    public OperationResult<User> Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
        {
            return OperationResult<User>.Fail("Please enter both the username and the password.");
        }

        return Run(() =>
        {
            var user = _users.GetByUsername(username.Trim());
            if (user == null || !PasswordHasher.Verify(password, user.PasswordHash))
            {
                return OperationResult<User>.Fail("Invalid username or password.");
            }

            if (!user.IsActive)
            {
                return OperationResult<User>.Fail("This account is deactivated. Please contact the Administrator.");
            }

            Session.SignIn(user);
            return OperationResult<User>.Ok(user, $"Welcome, {user.FullName}.");
        });
    }

    public void Logout()
    {
        Session.SignOut();
    }

    public OperationResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
        {
            return OperationResult.Fail("Please fill in all the password fields.");
        }

        if (newPassword.Length < PasswordHasher.MinimumLength)
        {
            return OperationResult.Fail($"The new password must have at least {PasswordHasher.MinimumLength} characters.");
        }

        if (newPassword != confirmPassword)
        {
            return OperationResult.Fail("The new password and the confirmation do not match.");
        }

        return Run(() =>
        {
            var user = _users.GetById(CurrentUser.UserID);
            if (user == null || !PasswordHasher.Verify(currentPassword, user.PasswordHash))
            {
                return OperationResult.Fail("The current password is not correct.");
            }

            if (currentPassword == newPassword)
            {
                return OperationResult.Fail("The new password must be different from the current password.");
            }

            _users.UpdatePassword(user.UserID, PasswordHasher.Hash(newPassword));
            return OperationResult.Ok("Your password has been changed successfully.");
        });
    }
}
