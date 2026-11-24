using ParkingLotManagementSystem.Controllers;
using ParkingLotManagementSystem.Models;

namespace ParkingLotManagementSystem.Views;

public partial class UserManagementForm : Form
{
    private readonly UserController _controller = new();

    public UserManagementForm()
    {
        InitializeComponent();
        dgvUsers.AutoGenerateColumns = false;
    }

    private void UserManagementForm_Load(object? sender, EventArgs e) => LoadUsers();

    private void btnSearch_Click(object? sender, EventArgs e) => LoadUsers();

    private void btnShowAll_Click(object? sender, EventArgs e)
    {
        txtSearch.Clear();
        LoadUsers();
    }

    private void txtSearch_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            LoadUsers();
        }
    }

    private void dgvUsers_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        UiTheme.FormatStatusCell(dgvUsers, e, nameof(User.StatusText));
    }

    private void dgvUsers_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            EditSelected();
        }
    }

    private void btnAdd_Click(object? sender, EventArgs e) => OpenEditor(null);

    private void btnEdit_Click(object? sender, EventArgs e) => EditSelected();

    private void btnActivate_Click(object? sender, EventArgs e) => SetActive(true);

    private void btnDeactivate_Click(object? sender, EventArgs e) => SetActive(false);

    private void btnClose_Click(object? sender, EventArgs e) => Close();

    private void btnDelete_Click(object? sender, EventArgs e)
    {
        var user = RequireSelection();
        if (user == null || !UiTheme.Confirm(this, $"Are you sure you want to delete the account '{user.Username}' ({user.FullName})?"))
        {
            return;
        }

        if (UiTheme.ShowResult(this, _controller.Delete(user.UserID)))
        {
            LoadUsers();
        }
    }

    private void LoadUsers()
    {
        var users = _controller.Search(txtSearch.Text);
        dgvUsers.DataSource = users;
        lblCount.Text = $"{users.Count} user(s)  |  Active: {users.Count(u => u.IsActive)}";
    }

    private User? RequireSelection()
    {
        var user = UiTheme.SelectedItem<User>(dgvUsers);
        if (user == null)
        {
            UiTheme.ShowError(this, "Please select a user first.");
        }

        return user;
    }

    private void EditSelected()
    {
        var user = RequireSelection();
        if (user != null)
        {
            OpenEditor(user);
        }
    }

    private void OpenEditor(User? user)
    {
        using var form = new UserEditForm(user);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            LoadUsers();
        }
    }

    private void SetActive(bool active)
    {
        var user = RequireSelection();
        if (user != null && UiTheme.ShowResult(this, _controller.SetActive(user.UserID, active)))
        {
            LoadUsers();
        }
    }
}
