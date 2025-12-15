namespace helloworld
{
    partial class DangNhapViews
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DangNhapViews));
            tableLayoutPanel1 = new TableLayoutPanel();
            aloneTextBoxUsername = new ReaLTaiizor.Controls.AloneTextBox();
            aloneTextBoxMatKhau = new ReaLTaiizor.Controls.AloneTextBox();
            cyberButtonDangNhap = new ReaLTaiizor.Controls.CyberButton();
            linkLabelDangky = new LinkLabel();
            linkLabelQuenMatKhau = new LinkLabel();
            pictureBox1 = new PictureBox();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(aloneTextBoxUsername, 0, 1);
            tableLayoutPanel1.Controls.Add(aloneTextBoxMatKhau, 0, 2);
            tableLayoutPanel1.Controls.Add(cyberButtonDangNhap, 0, 3);
            tableLayoutPanel1.Controls.Add(linkLabelDangky, 0, 4);
            tableLayoutPanel1.Controls.Add(linkLabelQuenMatKhau, 0, 5);
            tableLayoutPanel1.Controls.Add(pictureBox1, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.Size = new Size(429, 474);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // aloneTextBoxUsername
            // 
            aloneTextBoxUsername.Anchor = AnchorStyles.None;
            aloneTextBoxUsername.BackColor = Color.Transparent;
            aloneTextBoxUsername.EnabledCalc = true;
            aloneTextBoxUsername.Font = new Font("Segoe UI", 9F);
            aloneTextBoxUsername.ForeColor = Color.FromArgb(124, 133, 142);
            aloneTextBoxUsername.Location = new Point(93, 157);
            aloneTextBoxUsername.MaxLength = 32767;
            aloneTextBoxUsername.MultiLine = false;
            aloneTextBoxUsername.Name = "aloneTextBoxUsername";
            aloneTextBoxUsername.ReadOnly = false;
            aloneTextBoxUsername.Size = new Size(242, 36);
            aloneTextBoxUsername.TabIndex = 0;
            aloneTextBoxUsername.Text = "Tên đăng nhập";
            aloneTextBoxUsername.TextAlign = HorizontalAlignment.Left;
            aloneTextBoxUsername.UseSystemPasswordChar = false;
            // 
            // aloneTextBoxMatKhau
            // 
            aloneTextBoxMatKhau.Anchor = AnchorStyles.None;
            aloneTextBoxMatKhau.BackColor = Color.Transparent;
            aloneTextBoxMatKhau.EnabledCalc = true;
            aloneTextBoxMatKhau.Font = new Font("Segoe UI", 9F);
            aloneTextBoxMatKhau.ForeColor = Color.FromArgb(124, 133, 142);
            aloneTextBoxMatKhau.Location = new Point(94, 207);
            aloneTextBoxMatKhau.MaxLength = 32767;
            aloneTextBoxMatKhau.MultiLine = false;
            aloneTextBoxMatKhau.Name = "aloneTextBoxMatKhau";
            aloneTextBoxMatKhau.ReadOnly = false;
            aloneTextBoxMatKhau.Size = new Size(240, 36);
            aloneTextBoxMatKhau.TabIndex = 1;
            aloneTextBoxMatKhau.Text = "Mật khẩu";
            aloneTextBoxMatKhau.TextAlign = HorizontalAlignment.Left;
            aloneTextBoxMatKhau.UseSystemPasswordChar = false;
            // 
            // cyberButtonDangNhap
            // 
            cyberButtonDangNhap.Alpha = 20;
            cyberButtonDangNhap.Anchor = AnchorStyles.None;
            cyberButtonDangNhap.BackColor = Color.Transparent;
            cyberButtonDangNhap.Background = true;
            cyberButtonDangNhap.Background_WidthPen = 4F;
            cyberButtonDangNhap.BackgroundPen = true;
            cyberButtonDangNhap.ColorBackground = SystemColors.Highlight;
            cyberButtonDangNhap.ColorBackground_1 = Color.FromArgb(37, 52, 68);
            cyberButtonDangNhap.ColorBackground_2 = Color.FromArgb(41, 63, 86);
            cyberButtonDangNhap.ColorBackground_Pen = Color.LightGray;
            cyberButtonDangNhap.ColorLighting = Color.FromArgb(29, 200, 238);
            cyberButtonDangNhap.ColorPen_1 = Color.FromArgb(37, 52, 68);
            cyberButtonDangNhap.ColorPen_2 = Color.FromArgb(41, 63, 86);
            cyberButtonDangNhap.CyberButtonStyle = ReaLTaiizor.Enum.Cyber.StateStyle.Custom;
            cyberButtonDangNhap.Effect_1 = true;
            cyberButtonDangNhap.Effect_1_ColorBackground = Color.FromArgb(29, 200, 238);
            cyberButtonDangNhap.Effect_1_Transparency = 25;
            cyberButtonDangNhap.Effect_2 = true;
            cyberButtonDangNhap.Effect_2_ColorBackground = Color.White;
            cyberButtonDangNhap.Effect_2_Transparency = 20;
            cyberButtonDangNhap.Font = new Font("Arial", 11F);
            cyberButtonDangNhap.ForeColor = Color.FromArgb(245, 245, 245);
            cyberButtonDangNhap.Lighting = false;
            cyberButtonDangNhap.LinearGradient_Background = false;
            cyberButtonDangNhap.LinearGradientPen = false;
            cyberButtonDangNhap.Location = new Point(143, 253);
            cyberButtonDangNhap.Name = "cyberButtonDangNhap";
            cyberButtonDangNhap.PenWidth = 15;
            cyberButtonDangNhap.Rounding = true;
            cyberButtonDangNhap.RoundingInt = 70;
            cyberButtonDangNhap.Size = new Size(142, 44);
            cyberButtonDangNhap.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            cyberButtonDangNhap.TabIndex = 2;
            cyberButtonDangNhap.Tag = "Cyber";
            cyberButtonDangNhap.TextButton = "Đăng nhập";
            cyberButtonDangNhap.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            cyberButtonDangNhap.Timer_Effect_1 = 5;
            cyberButtonDangNhap.Timer_RGB = 300;
            cyberButtonDangNhap.Click += cyberButtonDangNhap_Click_1;
            // 
            // linkLabelDangky
            // 
            linkLabelDangky.Anchor = AnchorStyles.None;
            linkLabelDangky.AutoSize = true;
            linkLabelDangky.Location = new Point(150, 315);
            linkLabelDangky.Name = "linkLabelDangky";
            linkLabelDangky.Size = new Size(129, 20);
            linkLabelDangky.TabIndex = 3;
            linkLabelDangky.TabStop = true;
            linkLabelDangky.Text = "Tạo tài khoản mới";
            // 
            // linkLabelQuenMatKhau
            // 
            linkLabelQuenMatKhau.Anchor = AnchorStyles.Top;
            linkLabelQuenMatKhau.AutoSize = true;
            linkLabelQuenMatKhau.Location = new Point(160, 350);
            linkLabelQuenMatKhau.Name = "linkLabelQuenMatKhau";
            linkLabelQuenMatKhau.Size = new Size(109, 20);
            linkLabelQuenMatKhau.TabIndex = 4;
            linkLabelQuenMatKhau.TabStop = true;
            linkLabelQuenMatKhau.Text = "Quên mật khẩu";
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.None;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(133, 14);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(163, 122);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // DangNhapViews
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(429, 474);
            Controls.Add(tableLayoutPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "DangNhapViews";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Đăng nhập";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private ReaLTaiizor.Controls.AloneTextBox aloneTextBoxUsername;
        private ReaLTaiizor.Controls.AloneTextBox aloneTextBoxMatKhau;
        private ReaLTaiizor.Controls.CyberButton cyberButtonDangNhap;
        private LinkLabel linkLabelDangky;
        private LinkLabel linkLabelQuenMatKhau;
        private PictureBox pictureBox1;
    }
}