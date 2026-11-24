using ParkingLotManagementSystem.Controllers;
using ParkingLotManagementSystem.Models;

namespace ParkingLotManagementSystem.Views;

public partial class RecordsForm : Form
{
    private readonly RecordController _controller = new();
    private bool _loaded;

    public RecordsForm()
    {
        InitializeComponent();
        dgvRecords.AutoGenerateColumns = false;
        cboStatus.SelectedIndex = 0;
        cboType.SelectedIndex = 0;
        btnDelete.Visible = _controller.CanDeleteRecords;
    }

    private void RecordsForm_Load(object? sender, EventArgs e)
    {
        _loaded = true;
        LoadRecords();
    }

    private void Filter_Changed(object? sender, EventArgs e) => LoadRecords();

    private void dtpDate_ValueChanged(object? sender, EventArgs e) => LoadRecords();

    private void chkDate_CheckedChanged(object? sender, EventArgs e)
    {
        dtpDate.Enabled = chkDate.Checked;
        LoadRecords();
    }

    private void txtSearch_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            LoadRecords();
        }
    }

    private void btnSearch_Click(object? sender, EventArgs e) => LoadRecords();

    private void btnReset_Click(object? sender, EventArgs e)
    {
        _loaded = false;
        txtSearch.Clear();
        chkDate.Checked = false;
        dtpDate.Value = DateTime.Today;
        cboStatus.SelectedIndex = 0;
        cboType.SelectedIndex = 0;
        _loaded = true;
        LoadRecords();
    }

    private void btnDetails_Click(object? sender, EventArgs e) => ShowDetails();

    private void dgvRecords_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            ShowDetails();
        }
    }

    private void dgvRecords_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        UiTheme.FormatStatusCell(dgvRecords, e, nameof(ParkingRecord.Status));
    }

    private void btnClose_Click(object? sender, EventArgs e) => Close();

    private void btnDelete_Click(object? sender, EventArgs e)
    {
        var record = UiTheme.SelectedItem<ParkingRecord>(dgvRecords);
        if (record == null)
        {
            UiTheme.ShowError(this, "Please select a record first.");
            return;
        }

        if (!record.IsParked)
        {
            UiTheme.ShowError(this, "A completed record cannot be deleted.");
            return;
        }

        if (!UiTheme.Confirm(this, $"Delete record {record.TicketNo} for vehicle {record.PlateNo}? Slot {record.SlotNo} will become Available."))
        {
            return;
        }

        if (UiTheme.ShowResult(this, _controller.Delete(record.RecordID)))
        {
            LoadRecords();
        }
    }

    private void LoadRecords()
    {
        if (!_loaded)
        {
            return;
        }

        var records = _controller.Search(
            txtSearch.Text,
            chkDate.Checked ? dtpDate.Value.Date : null,
            UiTheme.FilterValue(cboStatus),
            UiTheme.FilterValue(cboType));
        dgvRecords.DataSource = records;

        var parked = records.Count(r => r.IsParked);
        var income = records.Sum(r => r.Fee ?? 0m);
        lblSummary.Text = $"{records.Count} record(s)   |   Parked: {parked}   |   Completed: {records.Count - parked}   |   Fees: {UiTheme.Money(income)}";
    }

    private void ShowDetails()
    {
        var record = UiTheme.SelectedItem<ParkingRecord>(dgvRecords);
        if (record == null)
        {
            UiTheme.ShowError(this, "Please select a record first.");
            return;
        }

        var lines = new List<(string, string)>
        {
            ("Ticket No", record.TicketNo),
            ("Plate No", record.PlateNo),
            ("Vehicle Type", record.VehicleType),
            ("Owner", record.OwnerName),
            ("Phone", string.IsNullOrWhiteSpace(record.Phone) ? "-" : record.Phone),
            ("Slot", $"{record.SlotNo} ({record.Zone})"),
            ("Entry Time", record.EntryTime.ToString(UiTheme.FullDateTimeFormat)),
            ("Exit Time", record.ExitTime?.ToString(UiTheme.FullDateTimeFormat) ?? "-"),
            ("Hours", record.Hours?.ToString() ?? "-"),
            ("Rate per Hour", UiTheme.Money(record.RatePerHour)),
            ("Fee", UiTheme.Money(record.Fee)),
            ("Amount Paid", UiTheme.Money(record.AmountPaid)),
            ("Change", UiTheme.Money(record.Change)),
            ("Payment Method", record.PaymentMethod ?? "-"),
            ("Status", record.Status),
            ("Attendant", record.AttendantName)
        };

        using var form = new SlipForm("Parking Record", $"Record {record.TicketNo}", lines, string.Empty);
        form.ShowDialog(this);
    }
}
