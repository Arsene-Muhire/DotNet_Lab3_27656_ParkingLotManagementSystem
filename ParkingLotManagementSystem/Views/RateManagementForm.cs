using ParkingLotManagementSystem.Controllers;
using ParkingLotManagementSystem.Models;

namespace ParkingLotManagementSystem.Views;

public partial class RateManagementForm : Form
{
    private readonly RateController _controller = new();
    private List<Rate> _rates = new();

    public RateManagementForm()
    {
        InitializeComponent();
        dgvRates.AutoGenerateColumns = false;
        cboType.SelectedIndex = 0;

        if (!_controller.CanEditRates)
        {
            Text = "Parking Rates";
            lblTitle.Text = "Parking Rates";
            lblSubtitle.Text = "Current price per hour for each vehicle type (view only)";
            lblType.Visible = false;
            cboType.Visible = false;
            lblPrice.Visible = false;
            nudPrice.Visible = false;
            btnSave.Visible = false;

            var bottomMargin = ClientSize.Height - btnClose.Bottom;
            btnClose.Top = lblType.Top;
            ClientSize = new Size(ClientSize.Width, btnClose.Bottom + bottomMargin);
        }
    }

    private void RateManagementForm_Load(object? sender, EventArgs e)
    {
        LoadRates();
    }

    private void dgvRates_SelectionChanged(object? sender, EventArgs e)
    {
        if (UiTheme.SelectedItem<Rate>(dgvRates) is { } rate)
        {
            cboType.SelectedItem = rate.VehicleType;
        }
    }

    private void cboType_SelectedIndexChanged(object? sender, EventArgs e)
    {
        ShowSelectedPrice();
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        var type = cboType.SelectedItem as string ?? string.Empty;
        if (!UiTheme.Confirm(this, $"Set the rate for {type} to {UiTheme.Money(nudPrice.Value)} per hour?"))
        {
            return;
        }

        if (UiTheme.ShowResult(this, _controller.SaveRate(type, nudPrice.Value)))
        {
            LoadRates();
        }
    }

    private void btnClose_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private void LoadRates()
    {
        _rates = _controller.GetRates();
        dgvRates.DataSource = _rates;
        ShowSelectedPrice();
    }

    private void ShowSelectedPrice()
    {
        var type = cboType.SelectedItem as string;
        var rate = _rates.FirstOrDefault(r => r.VehicleType == type);
        nudPrice.Value = rate == null ? 0 : Math.Min(nudPrice.Maximum, rate.PricePerHour);
    }
}
