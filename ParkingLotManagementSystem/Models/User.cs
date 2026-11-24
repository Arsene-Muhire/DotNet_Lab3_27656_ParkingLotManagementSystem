namespace ParkingLotManagementSystem.Models;

public class User
{
    public int UserID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = Roles.Attendant;
    public bool IsActive { get; set; } = true;

    public bool IsAdministrator => Role == Roles.Administrator;
    public string StatusText => IsActive ? "Active" : "Inactive";
}
