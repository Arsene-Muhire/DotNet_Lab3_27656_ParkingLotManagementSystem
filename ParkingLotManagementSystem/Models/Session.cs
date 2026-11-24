namespace ParkingLotManagementSystem.Models;

public static class Session
{
    public static User? CurrentUser { get; private set; }

    public static bool IsLoggedIn => CurrentUser != null;
    public static bool IsAdministrator => CurrentUser?.IsAdministrator == true;

    public static void SignIn(User user) => CurrentUser = user;

    public static void SignOut() => CurrentUser = null;
}
