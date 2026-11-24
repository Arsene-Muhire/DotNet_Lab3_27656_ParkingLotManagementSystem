using ParkingLotManagementSystem.Controllers;

namespace ParkingLotManagementSystem.Views;

public static class UiTheme
{
    public static readonly Color Primary = Color.FromArgb(30, 58, 95);
    public static readonly Color Success = Color.FromArgb(39, 174, 96);
    public static readonly Color Danger = Color.FromArgb(192, 57, 43);
    public static readonly Color Warning = Color.FromArgb(211, 84, 0);
    public static readonly Color Neutral = Color.FromArgb(108, 122, 137);
    public static readonly Color TextDark = Color.FromArgb(44, 62, 80);
    public static readonly Font BoldFont = new("Segoe UI", 10F, FontStyle.Bold);

    public const string AllOption = "All";
    public const string DateTimeFormat = "yyyy-MM-dd HH:mm";
    public const string FullDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    public const string MoneyFormat = "#,##0.##";

    public static string Money(decimal value) => $"{value.ToString(MoneyFormat)} RWF";

    public static string Money(decimal? value) => value.HasValue ? Money(value.Value) : "-";

    public static string FormatDuration(TimeSpan duration)
    {
        if (duration < TimeSpan.Zero)
        {
            duration = TimeSpan.Zero;
        }

        return $"{(int)duration.TotalHours}h {duration.Minutes:D2}m";
    }

    public static Color StatusColor(string status)
    {
        return status switch
        {
            "Available" => Success,
            "Occupied" => Danger,
            "Out of Service" => Neutral,
            "Parked" => Warning,
            "Completed" => Success,
            "Active" => Success,
            "Inactive" => Danger,
            _ => TextDark
        };
    }

    public static void FormatStatusCell(DataGridView grid, DataGridViewCellFormattingEventArgs e, string propertyName)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.CellStyle == null)
        {
            return;
        }

        if (grid.Columns[e.ColumnIndex].DataPropertyName != propertyName || e.Value is not string status)
        {
            return;
        }

        var color = StatusColor(status);
        e.CellStyle.ForeColor = color;
        e.CellStyle.SelectionForeColor = color;
        e.CellStyle.Font = BoldFont;
    }

    public static string? FilterValue(ComboBox combo)
    {
        var value = combo.SelectedItem as string;
        return value == null || value == AllOption ? null : value;
    }

    public static T? SelectedItem<T>(DataGridView grid) where T : class
    {
        return grid.CurrentRow?.DataBoundItem as T;
    }

    public static void ShowError(IWin32Window owner, string message)
    {
        MessageBox.Show(owner, message, "Parking Lot Management System", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    public static void ShowInfo(IWin32Window owner, string message)
    {
        MessageBox.Show(owner, message, "Parking Lot Management System", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public static bool Confirm(IWin32Window owner, string message)
    {
        return MessageBox.Show(owner, message, "Please confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }

    public static bool ShowResult(IWin32Window owner, OperationResult result)
    {
        if (result.Success)
        {
            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                ShowInfo(owner, result.Message);
            }
        }
        else
        {
            ShowError(owner, result.Message);
        }

        return result.Success;
    }
}
