using ParkingLotManagementSystem.Controllers;
using ParkingLotManagementSystem.Models;

namespace ParkingLotManagementSystem.Views;

public partial class MainForm : Form
{
    private readonly DashboardController _dashboard = new();
    private readonly AuthController _auth = new();
    private int _secondsSinceRefresh;

    public MainForm()
    {
        InitializeComponent();
        dgvParked.AutoGenerateColumns = false;

        var user = Session.CurrentUser!;
        lblUser.Text = $"{user.FullName}  ({user.Role})";
        btnSlots.Text = user.IsAdministrator ? "   Manage Slots" : "   View Slots";
        btnRates.Text = user.IsAdministrator ? "   Manage Rates" : "   View Rates";
        btnUsers.Visible = user.IsAdministrator;
    }

    public bool LoggedOut { get; private set; }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        UpdateClock();
        RefreshDashboard();
        tmrClock.Start();
    }

    private void MainForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        tmrClock.Stop();
    }

    private void tmrClock_Tick(object? sender, EventArgs e)
    {
        UpdateClock();
        _secondsSinceRefresh++;
        if (_secondsSinceRefresh >= 30 && ContainsFocus)
        {
            RefreshDashboard();
        }
    }

    private void btnDashboard_Click(object? sender, EventArgs e) => RefreshDashboard();

    private void btnCheckIn_Click(object? sender, EventArgs e) => OpenDialog(new CheckInForm());

    private void btnCheckOut_Click(object? sender, EventArgs e) => OpenDialog(new CheckOutForm());

    private void btnRecords_Click(object? sender, EventArgs e) => OpenDialog(new RecordsForm());

    private void btnSlots_Click(object? sender, EventArgs e) => OpenDialog(new SlotManagementForm());

    private void btnRates_Click(object? sender, EventArgs e) => OpenDialog(new RateManagementForm());

    private void btnUsers_Click(object? sender, EventArgs e) => OpenDialog(new UserManagementForm());

    private void btnReports_Click(object? sender, EventArgs e) => OpenDialog(new ReportsForm());

    private void btnChangePassword_Click(object? sender, EventArgs e) => OpenDialog(new ChangePasswordForm());

    private void btnLogout_Click(object? sender, EventArgs e)
    {
        if (!UiTheme.Confirm(this, "Do you want to log out?"))
        {
            return;
        }

        _auth.Logout();
        LoggedOut = true;
        Close();
    }

    private void OpenDialog(Form form)
    {
        using (form)
        {
            form.ShowDialog(this);
        }

        RefreshDashboard();
    }

    private void RefreshDashboard()
    {
        var summary = _dashboard.GetSummary();
        lblTotalValue.Text = summary.TotalSlots.ToString();
        lblAvailableValue.Text = summary.AvailableSlots.ToString();
        lblOccupiedValue.Text = summary.OccupiedSlots.ToString();
        lblOutOfServiceValue.Text = summary.OutOfServiceSlots.ToString();
        lblParkedValue.Text = summary.VehiclesParked.ToString();
        lblIncomeValue.Text = UiTheme.Money(summary.IncomeToday);
        dgvParked.DataSource = _dashboard.GetCurrentlyParked();
        lblUpdated.Text = $"Last updated at {DateTime.Now:HH:mm:ss}";
        _secondsSinceRefresh = 0;
    }

    private void UpdateClock()
    {
        lblClock.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy   HH:mm:ss");
    }
}
