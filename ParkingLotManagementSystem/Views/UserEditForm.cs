using ParkingLotManagementSystem.Controllers;
using ParkingLotManagementSystem.Models;

namespace ParkingLotManagementSystem.Views;

public partial class UserEditForm : Form
{
    private readonly UserController _controller = new();
    private readonly User? _user;

    public UserEditForm()
    {
        InitializeComponent();
        cboRole.SelectedItem = Roles.Attendant;
    }

    public UserEditForm(User? user) : this()
    {
        _user = user;
        if (user == null)
        {
            return;
        }

        Text = "Edit User";
        lblTitle.Text = $"Edit Account: {user.Username}";
        lblSubtitle.Text = "Leave the password fields empty to keep the current password";
        lblPassword.Text = "New Password";
        lblConfirm.Text = "Confirm New Password";
        txtFullName.Text = user.FullName;
        txtUsername.Text = user.Username;
        cboRole.SelectedItem = user.Role;
        chkActive.Checked = user.IsActive;
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        var role = cboRole.SelectedItem as string ?? string.Empty;
        var result = _user == null
            ? _controller.Add(txtFullName.Text, txtUsername.Text, txtPassword.Text, txtConfirm.Text, role, chkActive.Checked)
            : _controller.Update(_user.UserID, txtFullName.Text, txtUsername.Text, role, chkActive.Checked, txtPassword.Text, txtConfirm.Text);

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
