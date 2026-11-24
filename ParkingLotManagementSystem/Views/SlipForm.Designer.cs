#nullable disable

namespace ParkingLotManagementSystem.Views
{
    partial class SlipForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            dgvLines = new DataGridView();
            colLabel = new DataGridViewTextBoxColumn();
            colValue = new DataGridViewTextBoxColumn();
            lblFooter = new Label();
            btnOk = new Button();
            pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLines).BeginInit();
            SuspendLayout();
            pnlHeader.BackColor = Color.FromArgb(30, 58, 95);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(440, 64);
            pnlHeader.TabIndex = 0;
            lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 6);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(400, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Entry Ticket";
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.FromArgb(200, 214, 229);
            lblSubtitle.Location = new Point(20, 36);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(400, 22);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Details";
            lblSubtitle.TextAlign = ContentAlignment.MiddleLeft;
            dgvLines.AllowUserToAddRows = false;
            dgvLines.AllowUserToDeleteRows = false;
            dgvLines.AllowUserToResizeColumns = false;
            dgvLines.AllowUserToResizeRows = false;
            dgvLines.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvLines.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLines.BackgroundColor = Color.White;
            dgvLines.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvLines.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLines.ColumnHeadersVisible = false;
            dgvLines.Columns.AddRange(new DataGridViewColumn[] { colLabel, colValue });
            dataGridViewCellStyle1.BackColor = Color.White;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle1.ForeColor = Color.FromArgb(44, 62, 80);
            dataGridViewCellStyle1.Padding = new Padding(8, 0, 4, 0);
            dataGridViewCellStyle1.SelectionBackColor = Color.White;
            dataGridViewCellStyle1.SelectionForeColor = Color.FromArgb(44, 62, 80);
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.False;
            dgvLines.DefaultCellStyle = dataGridViewCellStyle1;
            dgvLines.GridColor = Color.FromArgb(235, 238, 242);
            dgvLines.Location = new Point(24, 84);
            dgvLines.MultiSelect = false;
            dgvLines.Name = "dgvLines";
            dgvLines.ReadOnly = true;
            dgvLines.RowHeadersVisible = false;
            dgvLines.RowTemplate.Height = 32;
            dgvLines.ScrollBars = ScrollBars.None;
            dgvLines.Size = new Size(392, 300);
            dgvLines.TabIndex = 1;
            dgvLines.TabStop = false;
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(120, 130, 140);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(120, 130, 140);
            colLabel.DefaultCellStyle = dataGridViewCellStyle2;
            colLabel.FillWeight = 80F;
            colLabel.HeaderText = "Item";
            colLabel.Name = "colLabel";
            colLabel.ReadOnly = true;
            colLabel.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            colValue.DefaultCellStyle = dataGridViewCellStyle3;
            colValue.FillWeight = 120F;
            colValue.HeaderText = "Value";
            colValue.Name = "colValue";
            colValue.ReadOnly = true;
            colValue.SortMode = DataGridViewColumnSortMode.NotSortable;
            lblFooter.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblFooter.Font = new Font("Segoe UI", 9F);
            lblFooter.ForeColor = Color.FromArgb(120, 130, 140);
            lblFooter.Location = new Point(24, 394);
            lblFooter.Name = "lblFooter";
            lblFooter.Size = new Size(392, 40);
            lblFooter.TabIndex = 2;
            lblFooter.TextAlign = ContentAlignment.MiddleCenter;
            btnOk.Anchor = AnchorStyles.Bottom;
            btnOk.BackColor = Color.FromArgb(41, 128, 185);
            btnOk.Cursor = Cursors.Hand;
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnOk.ForeColor = Color.White;
            btnOk.Location = new Point(150, 442);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(140, 36);
            btnOk.TabIndex = 0;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = false;
            btnOk.Click += btnOk_Click;
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(244, 246, 249);
            CancelButton = btnOk;
            ClientSize = new Size(440, 498);
            Controls.Add(btnOk);
            Controls.Add(lblFooter);
            Controls.Add(dgvLines);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.FromArgb(44, 62, 80);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SlipForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Slip";
            Load += SlipForm_Load;
            Shown += SlipForm_Shown;
            pnlHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvLines).EndInit();
            ResumeLayout(false);
        }

        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private DataGridView dgvLines;
        private DataGridViewTextBoxColumn colLabel;
        private DataGridViewTextBoxColumn colValue;
        private Label lblFooter;
        private Button btnOk;
    }
}
