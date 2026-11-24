using MySql.Data.MySqlClient;
using ParkingLotManagementSystem.Helpers;
using ParkingLotManagementSystem.Models;
using ParkingLotManagementSystem.Models.Data;

namespace ParkingLotManagementSystem.Controllers;

public class UserController : BaseController
{
    private readonly UserRepository _users = new();

    public List<User> Search(string keyword)
    {
        return IsAdministrator ? _users.Search(keyword) : new List<User>();
    }

    public OperationResult Add(string fullName, string username, string password, string confirmPassword, string role, bool isActive)
    {
        if (!IsAdministrator)
        {
            return OperationResult.Fail(AdministratorOnlyMessage);
        }

        var validation = ValidateDetails(fullName, username, role);
        if (validation != null)
        {
            return OperationResult.Fail(validation);
        }

        var passwordError = ValidatePassword(password, confirmPassword);
        if (passwordError != null)
        {
            return OperationResult.Fail(passwordError);
        }

        return Run(() =>
        {
            var cleanUsername = username.Trim();
            if (_users.UsernameExists(cleanUsername, 0))
            {
                return OperationResult.Fail($"The username '{cleanUsername}' is already used by another account.");
            }

            try
            {
                _users.Insert(new User
                {
                    FullName = fullName.Trim(),
                    Username = cleanUsername,
                    PasswordHash = PasswordHasher.Hash(password),
                    Role = role,
                    IsActive = isActive
                });
            }
            catch (MySqlException ex) when (ex.IsUniqueViolation())
            {
                return OperationResult.Fail($"The username '{cleanUsername}' is already used by another account.");
            }

            return OperationResult.Ok("The user account has been created.");
        });
    }

    public OperationResult Update(int userId, string fullName, string username, string role, bool isActive, string newPassword, string confirmPassword)
    {
        if (!IsAdministrator)
        {
            return OperationResult.Fail(AdministratorOnlyMessage);
        }

        var validation = ValidateDetails(fullName, username, role);
        if (validation != null)
        {
            return OperationResult.Fail(validation);
        }

        var changePassword = !string.IsNullOrEmpty(newPassword) || !string.IsNullOrEmpty(confirmPassword);
        if (changePassword)
        {
            var passwordError = ValidatePassword(newPassword, confirmPassword);
            if (passwordError != null)
            {
                return OperationResult.Fail(passwordError);
            }
        }

        return Run(() =>
        {
            var user = _users.GetById(userId);
            if (user == null)
            {
                return OperationResult.Fail("The selected user no longer exists.");
            }

            var isSelf = userId == CurrentUser.UserID;
            if (isSelf && (!isActive || role != Roles.Administrator))
            {
                return OperationResult.Fail("You cannot deactivate your own account or remove your own Administrator role.");
            }

            var losesAdminAccess = user.IsAdministrator && user.IsActive && (role != Roles.Administrator || !isActive);
            if (losesAdminAccess && _users.CountActiveAdministrators(userId) == 0)
            {
                return OperationResult.Fail("The system must keep at least one active Administrator.");
            }

            var cleanUsername = username.Trim();
            if (_users.UsernameExists(cleanUsername, userId))
            {
                return OperationResult.Fail($"The username '{cleanUsername}' is already used by another account.");
            }

            user.FullName = fullName.Trim();
            user.Username = cleanUsername;
            user.Role = role;
            user.IsActive = isActive;

            try
            {
                _users.Update(user);
            }
            catch (MySqlException ex) when (ex.IsUniqueViolation())
            {
                return OperationResult.Fail($"The username '{cleanUsername}' is already used by another account.");
            }

            if (changePassword)
            {
                _users.UpdatePassword(userId, PasswordHasher.Hash(newPassword));
            }

            if (isSelf)
            {
                Session.SignIn(user);
            }

            return OperationResult.Ok("The user account has been updated.");
        });
    }

    public OperationResult SetActive(int userId, bool isActive)
    {
        if (!IsAdministrator)
        {
            return OperationResult.Fail(AdministratorOnlyMessage);
        }

        if (userId == CurrentUser.UserID && !isActive)
        {
            return OperationResult.Fail("You cannot deactivate your own account.");
        }

        return Run(() =>
        {
            var user = _users.GetById(userId);
            if (user == null)
            {
                return OperationResult.Fail("The selected user no longer exists.");
            }

            if (user.IsActive == isActive)
            {
                return OperationResult.Fail($"This account is already {(isActive ? "active" : "inactive")}.");
            }

            if (!isActive && user.IsAdministrator && _users.CountActiveAdministrators(userId) == 0)
            {
                return OperationResult.Fail("The system must keep at least one active Administrator.");
            }

            _users.SetActive(userId, isActive);
            return OperationResult.Ok($"The account '{user.Username}' has been {(isActive ? "activated" : "deactivated")}.");
        });
    }

    public OperationResult Delete(int userId)
    {
        if (!IsAdministrator)
        {
            return OperationResult.Fail(AdministratorOnlyMessage);
        }

        if (userId == CurrentUser.UserID)
        {
            return OperationResult.Fail("You cannot delete your own account.");
        }

        return Run(() =>
        {
            var user = _users.GetById(userId);
            if (user == null)
            {
                return OperationResult.Fail("The selected user no longer exists.");
            }

            if (_users.HasParkingRecords(userId))
            {
                return OperationResult.Fail("This user has handled parking records and cannot be deleted. Deactivate the account instead.");
            }

            if (user.IsAdministrator && user.IsActive && _users.CountActiveAdministrators(userId) == 0)
            {
                return OperationResult.Fail("The system must keep at least one active Administrator.");
            }

            try
            {
                _users.Delete(userId);
            }
            catch (MySqlException ex) when (ex.IsForeignKeyViolation())
            {
                return OperationResult.Fail("This user is linked to parking records and cannot be deleted. Deactivate the account instead.");
            }

            return OperationResult.Ok($"The account '{user.Username}' has been deleted.");
        });
    }

    private static string? ValidateDetails(string fullName, string username, string role)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return "Please enter the full name.";
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            return "Please enter the username.";
        }

        var cleanUsername = username.Trim();
        if (cleanUsername.Length < 3 || cleanUsername.Length > 50 || cleanUsername.Any(char.IsWhiteSpace))
        {
            return "The username must have 3 to 50 characters and must not contain spaces.";
        }

        if (!Roles.All.Contains(role))
        {
            return "Please select a valid role.";
        }

        return null;
    }

    private static string? ValidatePassword(string password, string confirmPassword)
    {
        if (string.IsNullOrEmpty(password) || password.Length < PasswordHasher.MinimumLength)
        {
            return $"The password must have at least {PasswordHasher.MinimumLength} characters.";
        }

        if (password != confirmPassword)
        {
            return "The password and the confirmation do not match.";
        }

        return null;
    }
}
