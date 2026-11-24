using ParkingLotManagementSystem.Controllers;
using ParkingLotManagementSystem.Models;

namespace ParkingLotManagementSystem.Views;

public partial class CheckOutForm : Form
{
    private readonly CheckOutController _controller = new();
    private ParkingRecord? _record;
    private CheckOutQuote? _quote;

    public CheckOutForm()
    {
        InitializeComponent();
    }

    private Label[] ValueLabels => new[]
    {
        lblTicketValue, lblPlateValue, lblVehicleTypeValue, lblOwnerValue, lblPhoneValue, lblSlotValue,
        lblEntryValue, lblCurrentValue, lblDurationValue, lblHoursValue, lblRateValue, lblFeeValue
    };

    private void CheckOutForm_Load(object? sender, EventArgs e) => ClearForm();

    private void CheckOutForm_Shown(object? sender, EventArgs e) => txtSearch.Focus();

    private void CheckOutForm_FormClosed(object? sender, FormClosedEventArgs e) => tmrRefresh.Stop();

    private void tmrRefresh_Tick(object? sender, EventArgs e) => UpdateQuote();

    private void txtSearch_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            FindVehicle();
        }
    }

    private void btnFind_Click(object? sender, EventArgs e) => FindVehicle();

    private void nudAmount_ValueChanged(object? sender, EventArgs e) => UpdateChange();

    private void nudAmount_KeyUp(object? sender, KeyEventArgs e) => UpdateChange();

    private void btnClear_Click(object? sender, EventArgs e) => ClearForm();

    private void btnClose_Click(object? sender, EventArgs e) => Close();

    private void btnComplete_Click(object? sender, EventArgs e)
    {
        if (_record == null)
        {
            UiTheme.ShowError(this, "Please find a parked vehicle first.");
            return;
        }

        var method = cboMethod.SelectedItem as string ?? string.Empty;
        var result = _controller.CompleteCheckOut(_record.RecordID, nudAmount.Value, method);
        if (!result.Success || result.Data == null)
        {
            UiTheme.ShowError(this, result.Message);
            UpdateQuote();
            return;
        }

        var record = result.Data;
        var lines = new List<(string, string)>
        {
            ("Ticket No", record.TicketNo),
            ("Plate No", record.PlateNo),
            ("Slot", record.SlotNo),
            ("Entry Time", record.EntryTime.ToString(UiTheme.FullDateTimeFormat)),
            ("Exit Time", record.ExitTime?.ToString(UiTheme.FullDateTimeFormat) ?? "-"),
            ("Hours", $"{record.Hours} hour(s)"),
            ("Rate per Hour", UiTheme.Money(record.RatePerHour)),
            ("Fee", UiTheme.Money(record.Fee)),
            ("Amount Paid", UiTheme.Money(record.AmountPaid)),
            ("Change", UiTheme.Money(record.Change)),
            ("Payment Method", record.PaymentMethod ?? "-"),
            ("Attendant", record.AttendantName)
        };

        using (var receipt = new SlipForm("Payment Receipt", result.Message, lines, "Thank you for parking with us."))
        {
            receipt.ShowDialog(this);
        }

        ClearForm();
    }

    private void FindVehicle()
    {
        var result = _controller.FindParkedVehicle(txtSearch.Text);
        if (!result.Success || result.Data == null)
        {
            ClearDetails();
            UiTheme.ShowError(this, result.Message);
            txtSearch.Focus();
            txtSearch.SelectAll();
            return;
        }

        _record = result.Data;
        lblTicketValue.Text = _record.TicketNo;
        lblPlateValue.Text = _record.PlateNo;
        lblVehicleTypeValue.Text = _record.VehicleType;
        lblOwnerValue.Text = _record.OwnerName;
        lblPhoneValue.Text = string.IsNullOrWhiteSpace(_record.Phone) ? "-" : _record.Phone;
        lblSlotValue.Text = $"{_record.SlotNo} ({_record.Zone})";
        lblEntryValue.Text = _record.EntryTime.ToString(UiTheme.FullDateTimeFormat);
        lblRateValue.Text = UiTheme.Money(_record.RatePerHour);

        UpdateQuote();
        nudAmount.Value = Math.Min(nudAmount.Maximum, _quote?.Fee ?? 0);
        btnComplete.Enabled = true;
        tmrRefresh.Start();
        nudAmount.Focus();
        nudAmount.Select(0, nudAmount.Text.Length);
    }

    private void UpdateQuote()
    {
        if (_record == null)
        {
            return;
        }

        _quote = _controller.CalculateQuote(_record, DateTime.Now);
        lblCurrentValue.Text = _quote.ExitTime.ToString(UiTheme.FullDateTimeFormat);
        lblDurationValue.Text = UiTheme.FormatDuration(_quote.Duration);
        lblHoursValue.Text = $"{_quote.Hours} hour(s)";
        lblFeeValue.Text = $"{_quote.Hours} x {UiTheme.Money(_quote.RatePerHour)} = {UiTheme.Money(_quote.Fee)}";
        lblFeeAmount.Text = UiTheme.Money(_quote.Fee);
        UpdateChange();
    }

    private void UpdateChange()
    {
        if (_quote == null)
        {
            lblChange.Text = "-";
            return;
        }

        var difference = nudAmount.Value - _quote.Fee;
        if (difference < 0)
        {
            lblChange.ForeColor = UiTheme.Danger;
            lblChange.Text = $"Short by {UiTheme.Money(-difference)}";
        }
        else
        {
            lblChange.ForeColor = UiTheme.Success;
            lblChange.Text = UiTheme.Money(difference);
        }
    }

    private void ClearDetails()
    {
        tmrRefresh.Stop();
        _record = null;
        _quote = null;
        foreach (var label in ValueLabels)
        {
            label.Text = "-";
        }

        lblFeeAmount.Text = "-";
        lblChange.ForeColor = UiTheme.Success;
        lblChange.Text = "-";
        nudAmount.Value = 0;
        btnComplete.Enabled = false;
    }

    private void ClearForm()
    {
        ClearDetails();
        txtSearch.Clear();
        cboMethod.SelectedIndex = 0;
        txtSearch.Focus();
    }
}
