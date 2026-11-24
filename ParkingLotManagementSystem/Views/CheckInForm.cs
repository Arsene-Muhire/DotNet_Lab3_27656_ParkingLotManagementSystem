using ParkingLotManagementSystem.Controllers;
using ParkingLotManagementSystem.Models;

namespace ParkingLotManagementSystem.Views;

public partial class CheckInForm : Form
{
    private readonly CheckInController _controller = new();
    private bool _loaded;

    public CheckInForm()
    {
        InitializeComponent();
    }

    private string SelectedType => cboType.SelectedItem as string ?? string.Empty;

    private void CheckInForm_Load(object? sender, EventArgs e)
    {
        cboType.SelectedItem = VehicleTypes.Car;
        _loaded = true;
        LoadSlots();
    }

    private void CheckInForm_Shown(object? sender, EventArgs e)
    {
        txtPlate.Focus();
    }

    private void cboType_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_loaded)
        {
            LoadSlots();
        }
    }

    private void txtPlate_Leave(object? sender, EventArgs e)
    {
        var vehicle = _controller.FindVehicle(txtPlate.Text);
        if (vehicle == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(txtOwner.Text))
        {
            txtOwner.Text = vehicle.OwnerName;
        }

        if (string.IsNullOrWhiteSpace(txtPhone.Text))
        {
            txtPhone.Text = vehicle.Phone;
        }

        if (SelectedType != vehicle.VehicleType)
        {
            cboType.SelectedItem = vehicle.VehicleType;
        }
    }

    private void lstSlots_DoubleClick(object? sender, EventArgs e) => CheckIn();

    private void btnCheckIn_Click(object? sender, EventArgs e) => CheckIn();

    private void btnClear_Click(object? sender, EventArgs e) => ClearForm();

    private void btnClose_Click(object? sender, EventArgs e) => Close();

    private void LoadSlots()
    {
        var slots = _controller.GetAvailableSlots(SelectedType);
        lstSlots.DataSource = slots;

        if (slots.Count == 0)
        {
            lblSlotMessage.ForeColor = UiTheme.Danger;
            lblSlotMessage.Text = $"No slot is available for {SelectedType} vehicles at the moment.";
        }
        else
        {
            lblSlotMessage.ForeColor = UiTheme.Success;
            lblSlotMessage.Text = $"{slots.Count} available slot(s) for {SelectedType}. Select one.";
            lstSlots.SelectedIndex = 0;
        }

        var rate = _controller.GetRate(SelectedType);
        lblRate.Text = rate == null
            ? $"No rate is set for {SelectedType}."
            : $"Current rate for {SelectedType}: {UiTheme.Money(rate.PricePerHour)} per hour (any part of an hour counts as a full hour)";
    }

    private void CheckIn()
    {
        var slot = lstSlots.SelectedItem as ParkingSlot;
        var result = _controller.CheckIn(txtPlate.Text, SelectedType, txtOwner.Text, txtPhone.Text, slot?.SlotID);
        if (!result.Success || result.Data == null)
        {
            UiTheme.ShowError(this, result.Message);
            LoadSlots();
            return;
        }

        var record = result.Data;
        var lines = new List<(string, string)>
        {
            ("Ticket No", record.TicketNo),
            ("Plate No", record.PlateNo),
            ("Vehicle Type", record.VehicleType),
            ("Owner", record.OwnerName),
            ("Slot", $"{record.SlotNo} ({record.Zone})"),
            ("Entry Time", record.EntryTime.ToString(UiTheme.FullDateTimeFormat)),
            ("Rate per Hour", UiTheme.Money(record.RatePerHour)),
            ("Attendant", record.AttendantName)
        };

        using (var ticket = new SlipForm("Entry Ticket", result.Message, lines, "Keep this ticket number. It is needed at check-out."))
        {
            ticket.ShowDialog(this);
        }

        ClearForm();
    }

    private void ClearForm()
    {
        txtPlate.Clear();
        txtOwner.Clear();
        txtPhone.Clear();
        LoadSlots();
        txtPlate.Focus();
    }
}
