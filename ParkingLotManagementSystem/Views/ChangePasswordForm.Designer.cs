#nullable disable

namespace ParkingLotManagementSystem.Views
{
    partial class ChangePasswordForm
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
            lblCurrent = new Label();
            txtCurrent = new TextBox();
            lblNew = new Label();
            txtNew = new TextBox();
            lblConfirm = new Label();
            txtConfirm = new TextBox();
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
            lblTitle.Text = "Change Password";
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.FromArgb(200, 214, 229);
            lblSubtitle.Location = new Point(20, 36);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(390, 22);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Account";
            lblSubtitle.TextAlign = ContentAlignment.MiddleLeft;
            lblCurrent.Location = new Point(30, 80);
            lblCurrent.Name = "lblCurrent";
            lblCurrent.Size = new Size(360, 23);
            lblCurrent.TabIndex = 1;
            lblCurrent.Text = "Current Password";
            lblCurrent.TextAlign = ContentAlignment.MiddleLeft;
            txtCurrent.Location = new Point(30, 104);
            txtCurrent.Name = "txtCurrent";
            txtCurrent.Size = new Size(360, 25);
            txtCurrent.TabIndex = 2;
            txtCurrent.UseSystemPasswordChar = true;
            lblNew.Location = new Point(30, 140);
            lblNew.Name = "lblNew";
            lblNew.Size = new Size(360, 23);
            lblNew.TabIndex = 3;
            lblNew.Text = "New Password (at least 6 characters)";
            lblNew.TextAlign = ContentAlignment.MiddleLeft;
            txtNew.Location = new Point(30, 164);
            txtNew.Name = "txtNew";
            txtNew.Size = new Size(360, 25);
            txtNew.TabIndex = 4;
            txtNew.UseSystemPasswordChar = true;
            lblConfirm.Location = new Point(30, 200);
            lblConfirm.Name = "lblConfirm";
            lblConfirm.Size = new Size(360, 23);
            lblConfirm.TabIndex = 5;
            lblConfirm.Text = "Confirm New Password";
            lblConfirm.TextAlign = ContentAlignment.MiddleLeft;
            txtConfirm.Location = new Point(30, 224);
            txtConfirm.Name = "txtConfirm";
            txtConfirm.Size = new Size(360, 25);
            txtConfirm.TabIndex = 6;
            txtConfirm.UseSystemPasswordChar = true;
            btnSave.BackColor = Color.FromArgb(39, 174, 96);
            btnSave.Cursor = Cursors.Hand;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(30, 284);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(175, 36);
            btnSave.TabIndex = 7;
            btnSave.Text = "Change Password";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            btnCancel.BackColor = Color.FromArgb(108, 122, 137);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(215, 284);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(175, 36);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            AcceptButton = btnSave;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(244, 246, 249);
            CancelButton = btnCancel;
            ClientSize = new Size(420, 350);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(txtConfirm);
            Controls.Add(lblConfirm);
            Controls.Add(txtNew);
            Controls.Add(lblNew);
            Controls.Add(txtCurrent);
            Controls.Add(lblCurrent);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.FromArgb(44, 62, 80);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChangePasswordForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Change Password";
            pnlHeader.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblCurrent;
        private TextBox txtCurrent;
        private Label lblNew;
        private TextBox txtNew;
        private Label lblConfirm;
        private TextBox txtConfirm;
        private Button btnSave;
        private Button btnCancel;
    }
}
