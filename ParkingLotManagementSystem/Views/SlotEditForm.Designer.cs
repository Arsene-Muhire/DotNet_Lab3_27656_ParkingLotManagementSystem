#nullable disable

namespace ParkingLotManagementSystem.Views
{
    partial class SlotEditForm
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
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            lblSlotNo = new Label();
            txtSlotNo = new TextBox();
            lblType = new Label();
            cboType = new ComboBox();
            lblZone = new Label();
            cboZone = new ComboBox();
            lblStatus = new Label();
            cboStatus = new ComboBox();
            btnSave = new Button();
            btnCancel = new Button();
            pnlHeader.SuspendLayout();
            SuspendLayout();
            pnlHeader.BackColor = Color.FromArgb(30, 58, 95);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(420, 64);
            pnlHeader.TabIndex = 0;
            lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 6);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(390, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Add Parking Slot";
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.FromArgb(200, 214, 229);
            lblSubtitle.Location = new Point(20, 36);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(390, 22);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Fields marked * are required";
            lblSubtitle.TextAlign = ContentAlignment.MiddleLeft;
            lblSlotNo.Location = new Point(30, 80);
            lblSlotNo.Name = "lblSlotNo";
            lblSlotNo.Size = new Size(360, 23);
            lblSlotNo.TabIndex = 1;
            lblSlotNo.Text = "Slot Number *";
            lblSlotNo.TextAlign = ContentAlignment.MiddleLeft;
            txtSlotNo.CharacterCasing = CharacterCasing.Upper;
            txtSlotNo.Location = new Point(30, 104);
            txtSlotNo.MaxLength = 20;
            txtSlotNo.Name = "txtSlotNo";
            txtSlotNo.Size = new Size(360, 25);
            txtSlotNo.TabIndex = 2;
            lblType.Location = new Point(30, 140);
            lblType.Name = "lblType";
            lblType.Size = new Size(360, 23);
            lblType.TabIndex = 3;
            lblType.Text = "Vehicle Type *";
            lblType.TextAlign = ContentAlignment.MiddleLeft;
            cboType.DropDownStyle = ComboBoxStyle.DropDownList;
            cboType.Items.AddRange(new object[] { "Motorcycle", "Car", "Bus/Truck" });
            cboType.Location = new Point(30, 164);
            cboType.Name = "cboType";
            cboType.Size = new Size(360, 25);
            cboType.TabIndex = 4;
            lblZone.Location = new Point(30, 200);
            lblZone.Name = "lblZone";
            lblZone.Size = new Size(360, 23);
            lblZone.TabIndex = 5;
            lblZone.Text = "Zone / Floor *";
            lblZone.TextAlign = ContentAlignment.MiddleLeft;
            cboZone.Location = new Point(30, 224);
            cboZone.MaxLength = 50;
            cboZone.Name = "cboZone";
            cboZone.Size = new Size(360, 25);
            cboZone.TabIndex = 6;
            lblStatus.Location = new Point(30, 260);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(360, 23);
            lblStatus.TabIndex = 7;
            lblStatus.Text = "Status";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatus.Items.AddRange(new object[] { "Available", "Out of Service" });
            cboStatus.Location = new Point(30, 284);
            cboStatus.Name = "cboStatus";
            cboStatus.Size = new Size(360, 25);
            cboStatus.TabIndex = 8;
            btnSave.BackColor = Color.FromArgb(39, 174, 96);
            btnSave.Cursor = Cursors.Hand;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(30, 340);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(175, 36);
            btnSave.TabIndex = 9;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            btnCancel.BackColor = Color.FromArgb(108, 122, 137);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(215, 340);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(175, 36);
            btnCancel.TabIndex = 10;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            AcceptButton = btnSave;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(244, 246, 249);
            CancelButton = btnCancel;
            ClientSize = new Size(420, 400);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(cboStatus);
            Controls.Add(lblStatus);
            Controls.Add(cboZone);
            Controls.Add(lblZone);
            Controls.Add(cboType);
            Controls.Add(lblType);
            Controls.Add(txtSlotNo);
            Controls.Add(lblSlotNo);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.FromArgb(44, 62, 80);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SlotEditForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Add Slot";
            pnlHeader.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblSlotNo;
        private TextBox txtSlotNo;
        private Label lblType;
        private ComboBox cboType;
        private Label lblZone;
        private ComboBox cboZone;
        private Label lblStatus;
        private ComboBox cboStatus;
        private Button btnSave;
        private Button btnCancel;
    }
}
