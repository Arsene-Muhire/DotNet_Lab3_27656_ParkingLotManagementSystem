using ParkingLotManagementSystem.Controllers;
using ParkingLotManagementSystem.Models;

namespace ParkingLotManagementSystem.Views;

public partial class SlotManagementForm : Form
{
    private readonly SlotController _controller = new();
    private readonly bool _canManage;
    private bool _loaded;

    public SlotManagementForm()
    {
        InitializeComponent();
        dgvSlots.AutoGenerateColumns = false;
        cboStatus.SelectedIndex = 0;
        cboType.SelectedIndex = 0;
        cboZone.SelectedIndex = 0;

        _canManage = Session.IsAdministrator;
        if (!_canManage)
        {
            Text = "Parking Slots";
            lblTitle.Text = "Parking Slots";
            lblSubtitle.Text = "View the status of every parking slot";
            btnAdd.Visible = false;
            btnEdit.Visible = false;
            btnToggle.Visible = false;
            btnDelete.Visible = false;
            lblSummary.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            lblSummary.TextAlign = ContentAlignment.MiddleLeft;
            lblSummary.Left = btnAdd.Left;
        }
    }

    private void SlotManagementForm_Load(object? sender, EventArgs e)
    {
        LoadZones();
        _loaded = true;
        LoadSlots();
    }

    private void Filter_Changed(object? sender, EventArgs e) => LoadSlots();

    private void btnReset_Click(object? sender, EventArgs e)
    {
        _loaded = false;
        cboStatus.SelectedIndex = 0;
        cboType.SelectedIndex = 0;
        cboZone.SelectedIndex = 0;
        txtSlotNo.Clear();
        _loaded = true;
        LoadSlots();
    }

    private void dgvSlots_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        UiTheme.FormatStatusCell(dgvSlots, e, nameof(ParkingSlot.Status));
    }

    private void dgvSlots_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (_canManage && e.RowIndex >= 0)
        {
            EditSlot();
        }
    }

    private void btnAdd_Click(object? sender, EventArgs e)
    {
        using var form = new SlotEditForm(null, _controller.GetZones());
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            Reload();
        }
    }

    private void btnEdit_Click(object? sender, EventArgs e) => EditSlot();

    private void btnToggle_Click(object? sender, EventArgs e)
    {
        var slot = RequireSelection();
        if (slot != null && UiTheme.ShowResult(this, _controller.ToggleOutOfService(slot.SlotID)))
        {
            LoadSlots();
        }
    }

    private void btnDelete_Click(object? sender, EventArgs e)
    {
        var slot = RequireSelection();
        if (slot == null || !UiTheme.Confirm(this, $"Are you sure you want to delete slot {slot.SlotNo}?"))
        {
            return;
        }

        if (UiTheme.ShowResult(this, _controller.Delete(slot.SlotID)))
        {
            Reload();
        }
    }

    private void btnClose_Click(object? sender, EventArgs e) => Close();

    private ParkingSlot? RequireSelection()
    {
        var slot = UiTheme.SelectedItem<ParkingSlot>(dgvSlots);
        if (slot == null)
        {
            UiTheme.ShowError(this, "Please select a slot first.");
        }

        return slot;
    }

    private void EditSlot()
    {
        var slot = RequireSelection();
        if (slot == null)
        {
            return;
        }

        using var form = new SlotEditForm(slot, _controller.GetZones());
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            Reload();
        }
    }

    private void LoadZones()
    {
        var wasLoaded = _loaded;
        _loaded = false;
        var selected = cboZone.SelectedItem as string;
        cboZone.Items.Clear();
        cboZone.Items.Add(UiTheme.AllOption);
        foreach (var zone in _controller.GetZones())
        {
            cboZone.Items.Add(zone);
        }

        cboZone.SelectedItem = selected != null && cboZone.Items.Contains(selected) ? selected : UiTheme.AllOption;
        _loaded = wasLoaded;
    }

    private void LoadSlots()
    {
        if (!_loaded)
        {
            return;
        }

        var slots = _controller.GetSlots(UiTheme.FilterValue(cboStatus), UiTheme.FilterValue(cboType), UiTheme.FilterValue(cboZone), txtSlotNo.Text);
        dgvSlots.DataSource = slots;
        lblSummary.Text =
            $"{slots.Count} slot(s)  |  Available: {slots.Count(s => s.Status == SlotStatuses.Available)}  |  " +
            $"Occupied: {slots.Count(s => s.Status == SlotStatuses.Occupied)}  |  Out of Service: {slots.Count(s => s.Status == SlotStatuses.OutOfService)}";
    }

    private void Reload()
    {
        LoadZones();
        LoadSlots();
    }
}
