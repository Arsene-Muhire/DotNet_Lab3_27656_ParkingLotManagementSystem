using ParkingLotManagementSystem.Models.Data;
using ParkingLotManagementSystem.Views;

namespace ParkingLotManagementSystem;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => ShowUnexpectedError(e.Exception);
        ApplicationConfiguration.Initialize();

        try
        {
            DatabaseInitializer.Initialize();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "The application could not connect to the MySQL database.\n\n" +
                "Please open the XAMPP Control Panel, start MySQL, and check the connection string in the configuration file.\n\n" +
                "Details: " + ex.Message,
                "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        while (true)
        {
            using (var login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
            }

            using var main = new MainForm();
            Application.Run(main);
            if (!main.LoggedOut)
            {
                return;
            }
        }
    }

    private static void ShowUnexpectedError(Exception exception)
    {
        MessageBox.Show(
            "An unexpected error occurred:\n\n" + exception.Message,
            "Parking Lot Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
