using ParkingLotManagementSystem.Controllers;
using ParkingLotManagementSystem.Models;

namespace ParkingLotManagementSystem.Views;

public partial class SlotEditForm : Form
{
    private readonly SlotController _controller = new();
    private readonly ParkingSlot? _slot;

    public SlotEditForm()
    {
        InitializeComponent();
    }

    public SlotEditForm(ParkingSlot? slot, IEnumerable<string> zones) : this()
    {
        _slot = slot;
        cboZone.Items.AddRange(zones.Cast<object>().ToArray());
        cboType.SelectedIndex = 1;
        cboStatus.SelectedIndex = 0;

        if (slot == null)
        {
            cboStatus.Enabled = false;
            return;
        }

        var isOccupied = slot.Status == SlotStatuses.Occupied;
        Text = "Edit Slot";
        lblTitle.Text = $"Edit Slot {slot.SlotNo}";
        lblSubtitle.Text = isOccupied
            ? "This slot is Occupied: only the number and zone can change"
            : "Fields marked * are required";

        txtSlotNo.Text = slot.SlotNo;
        cboType.SelectedItem = slot.VehicleType;
        cboZone.Text = slot.Zone;

        if (isOccupied)
        {
            cboStatus.Items.Clear();
            cboStatus.Items.Add(SlotStatuses.Occupied);
            cboStatus.SelectedIndex = 0;
            cboStatus.Enabled = false;
            cboType.Enabled = false;
        }
        else
        {
            cboStatus.SelectedItem = slot.Status;
        }
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        var type = cboType.SelectedItem as string ?? string.Empty;
        var status = cboStatus.SelectedItem as string ?? SlotStatuses.Available;
        var result = _slot == null
            ? _controller.Add(txtSlotNo.Text, type, cboZone.Text)
            : _controller.Update(_slot.SlotID, txtSlotNo.Text, type, cboZone.Text, status);

        if (UiTheme.ShowResult(this, result))
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    private void btnCancel_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
