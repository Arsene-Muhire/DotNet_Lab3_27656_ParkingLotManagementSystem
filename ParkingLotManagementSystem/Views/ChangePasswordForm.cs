using ParkingLotManagementSystem.Controllers;
using ParkingLotManagementSystem.Models;

namespace ParkingLotManagementSystem.Views;

public partial class ChangePasswordForm : Form
{
    private readonly AuthController _controller = new();

    public ChangePasswordForm()
    {
        InitializeComponent();
        lblSubtitle.Text = $"Account: {Session.CurrentUser?.Username}";
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        var result = _controller.ChangePassword(txtCurrent.Text, txtNew.Text, txtConfirm.Text);
        if (UiTheme.ShowResult(this, result))
        {
            Close();
            return;
        }

        txtCurrent.Clear();
        txtNew.Clear();
        txtConfirm.Clear();
        txtCurrent.Focus();
    }

    private void btnCancel_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
