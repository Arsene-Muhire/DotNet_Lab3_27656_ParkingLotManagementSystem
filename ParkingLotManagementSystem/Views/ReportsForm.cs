using ParkingLotManagementSystem.Controllers;

namespace ParkingLotManagementSystem.Views;

public partial class ReportsForm : Form
{
    private readonly ReportController _controller = new();
    private bool _loaded;

    public ReportsForm()
    {
        InitializeComponent();
        foreach (var grid in new[] { dgvParked, dgvDaily, dgvByType, dgvByMethod, dgvOccupancy })
        {
            grid.AutoGenerateColumns = false;
        }

        dtpDaily.Value = DateTime.Today;
        dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        dtpTo.Value = DateTime.Today;

        if (!_controller.CanViewAllReports)
        {
            lblSubtitle.Text = "Parked vehicles and daily income";
            tabReports.TabPages.Remove(tabPeriod);
            tabReports.TabPages.Remove(tabOccupancy);
        }
    }

    private void ReportsForm_Load(object? sender, EventArgs e)
    {
        _loaded = true;
        LoadParked();
        LoadDaily();
    }

    private void tabReports_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_loaded)
        {
            return;
        }

        var selected = tabReports.SelectedTab;
        if (selected == tabParked)
        {
            LoadParked();
        }
        else if (selected == tabDaily)
        {
            LoadDaily();
        }
        else if (selected == tabPeriod)
        {
            LoadPeriod();
        }
        else if (selected == tabOccupancy)
        {
            LoadOccupancy();
        }
    }

    private void btnParkedRefresh_Click(object? sender, EventArgs e) => LoadParked();

    private void btnDailyShow_Click(object? sender, EventArgs e) => LoadDaily();

    private void dtpDaily_ValueChanged(object? sender, EventArgs e)
    {
        if (_loaded)
        {
            LoadDaily();
        }
    }

    private void btnPeriodShow_Click(object? sender, EventArgs e) => LoadPeriod();

    private void btnOccupancyRefresh_Click(object? sender, EventArgs e) => LoadOccupancy();

    private void dgvParked_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex >= 0 && dgvParked.Columns[e.ColumnIndex] == colParkedDuration && e.Value is TimeSpan span)
        {
            e.Value = UiTheme.FormatDuration(span);
            e.FormattingApplied = true;
        }
    }

    private void LoadParked()
    {
        var records = _controller.GetCurrentlyParked();
        dgvParked.DataSource = records;
        lblParkedSummary.Text = $"{records.Count} vehicle(s) currently parked   |   As of {DateTime.Now:yyyy-MM-dd HH:mm}";
    }

    private void LoadDaily()
    {
        var report = _controller.GetDailyIncome(dtpDaily.Value);
        dgvDaily.DataSource = report.Records;
        lblDailySummary.Text = $"{report.Date:yyyy-MM-dd}   |   Vehicles: {report.VehicleCount}   |   Total Collected: {UiTheme.Money(report.TotalIncome)}";
    }

    private void LoadPeriod()
    {
        var result = _controller.GetPeriodIncome(dtpFrom.Value, dtpTo.Value);
        if (!result.Success || result.Data == null)
        {
            UiTheme.ShowError(this, result.Message);
            return;
        }

        var report = result.Data;
        dgvByType.DataSource = report.ByVehicleType;
        dgvByMethod.DataSource = report.ByPaymentMethod;
        lblPeriodSummary.Text = $"Vehicles: {report.TotalVehicles}   |   Total Income: {UiTheme.Money(report.TotalIncome)}";
    }

    private void LoadOccupancy()
    {
        var result = _controller.GetSlotOccupancy();
        if (!result.Success || result.Data == null)
        {
            UiTheme.ShowError(this, result.Message);
            return;
        }

        var rows = result.Data;
        dgvOccupancy.DataSource = rows;
        lblOccupancySummary.Text =
            $"Total: {rows.Sum(r => r.TotalSlots)}   |   Available: {rows.Sum(r => r.Available)}   |   " +
            $"Occupied: {rows.Sum(r => r.Occupied)}   |   Out of Service: {rows.Sum(r => r.OutOfService)}";
    }
}
