namespace ParkingLotManagementSystem.Views;

public partial class SlipForm : Form
{
    public SlipForm()
    {
        InitializeComponent();
    }

    public SlipForm(string title, string subtitle, IReadOnlyList<(string Label, string Value)> lines, string footer) : this()
    {
        Text = title;
        lblTitle.Text = title;
        lblSubtitle.Text = subtitle;
        lblFooter.Text = footer;
        foreach (var (label, value) in lines)
        {
            dgvLines.Rows.Add(label, value);
        }
    }

    private void SlipForm_Load(object? sender, EventArgs e)
    {
        var neededHeight = dgvLines.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + 2;
        ClientSize = new Size(ClientSize.Width, ClientSize.Height + neededHeight - dgvLines.Height);
    }

    private void SlipForm_Shown(object? sender, EventArgs e)
    {
        dgvLines.ClearSelection();
        btnOk.Focus();
    }

    private void btnOk_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
