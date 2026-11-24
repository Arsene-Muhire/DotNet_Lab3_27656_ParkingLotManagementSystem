using ParkingLotManagementSystem.Controllers;

namespace ParkingLotManagementSystem.Views;

public partial class LoginForm : Form
{
    private readonly AuthController _controller = new();

    public LoginForm()
    {
        InitializeComponent();
    }

    private void LoginForm_Shown(object? sender, EventArgs e)
    {
        txtUsername.Focus();
    }

    private void btnLogin_Click(object? sender, EventArgs e)
    {
        lblError.Text = string.Empty;
        Cursor = Cursors.WaitCursor;
        var result = _controller.Login(txtUsername.Text, txtPassword.Text);
        Cursor = Cursors.Default;

        if (result.Success)
        {
            DialogResult = DialogResult.OK;
            Close();
            return;
        }

        lblError.Text = result.Message;
        txtPassword.Clear();
        txtPassword.Focus();
    }

    private void btnExit_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
