namespace helloworld
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            panel1 = new Panel();
            pictureBox6 = new PictureBox();
            pictureBox5 = new PictureBox();
            pictureBox4 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            panelMenu = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            buttonDangXuat = new Button();
            buttonCaiDat = new Button();
            pictureBox8 = new PictureBox();
            buttonSanPham = new Button();
            buttonHoaDon = new Button();
            buttonKhachHang = new Button();
            button1 = new Button();
            panelDesktop = new Panel();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panelMenu.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox8).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(pictureBox6);
            panel1.Controls.Add(pictureBox5);
            panel1.Controls.Add(pictureBox4);
            panel1.Controls.Add(pictureBox3);
            panel1.Controls.Add(pictureBox2);
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1062, 77);
            panel1.TabIndex = 1;
            // 
            // pictureBox6
            // 
            pictureBox6.Dock = DockStyle.Left;
            pictureBox6.Image = Properties.Resources.tik_tok1;
            pictureBox6.Location = new Point(402, 0);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Padding = new Padding(25);
            pictureBox6.Size = new Size(76, 77);
            pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox6.TabIndex = 6;
            pictureBox6.TabStop = false;
            // 
            // pictureBox5
            // 
            pictureBox5.Dock = DockStyle.Left;
            pictureBox5.Image = Properties.Resources.facebook;
            pictureBox5.Location = new Point(326, 0);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Padding = new Padding(25);
            pictureBox5.Size = new Size(76, 77);
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.TabIndex = 5;
            pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Dock = DockStyle.Left;
            pictureBox4.Image = Properties.Resources.browser;
            pictureBox4.Location = new Point(250, 0);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Padding = new Padding(25);
            pictureBox4.Size = new Size(76, 77);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 4;
            pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Dock = DockStyle.Left;
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(0, 0);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Padding = new Padding(25);
            pictureBox3.Size = new Size(250, 77);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.TabIndex = 3;
            pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Dock = DockStyle.Right;
            pictureBox2.Image = Properties.Resources.notification;
            pictureBox2.Location = new Point(911, 0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Padding = new Padding(25);
            pictureBox2.Size = new Size(76, 77);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Right;
            pictureBox1.Image = Properties.Resources.boy;
            pictureBox1.Location = new Point(987, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Padding = new Padding(10);
            pictureBox1.Size = new Size(75, 77);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // panelMenu
            // 
            panelMenu.BackColor = SystemColors.Control;
            panelMenu.Controls.Add(tableLayoutPanel1);
            panelMenu.Controls.Add(buttonSanPham);
            panelMenu.Controls.Add(buttonHoaDon);
            panelMenu.Controls.Add(buttonKhachHang);
            panelMenu.Controls.Add(button1);
            panelMenu.Dock = DockStyle.Left;
            panelMenu.Location = new Point(0, 77);
            panelMenu.Name = "panelMenu";
            panelMenu.Size = new Size(250, 596);
            panelMenu.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(buttonDangXuat, 1, 1);
            tableLayoutPanel1.Controls.Add(buttonCaiDat, 1, 0);
            tableLayoutPanel1.Controls.Add(pictureBox8, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Bottom;
            tableLayoutPanel1.Location = new Point(0, 515);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(250, 81);
            tableLayoutPanel1.TabIndex = 5;
            // 
            // buttonDangXuat
            // 
            buttonDangXuat.Dock = DockStyle.Top;
            buttonDangXuat.FlatAppearance.BorderSize = 0;
            buttonDangXuat.FlatStyle = FlatStyle.Flat;
            buttonDangXuat.Location = new Point(53, 43);
            buttonDangXuat.Name = "buttonDangXuat";
            buttonDangXuat.Size = new Size(194, 31);
            buttonDangXuat.TabIndex = 6;
            buttonDangXuat.Text = "Đăng xuất";
            buttonDangXuat.TextAlign = ContentAlignment.MiddleLeft;
            buttonDangXuat.UseVisualStyleBackColor = true;
            // 
            // buttonCaiDat
            // 
            buttonCaiDat.Dock = DockStyle.Top;
            buttonCaiDat.FlatAppearance.BorderSize = 0;
            buttonCaiDat.FlatStyle = FlatStyle.Flat;
            buttonCaiDat.Location = new Point(53, 3);
            buttonCaiDat.Name = "buttonCaiDat";
            buttonCaiDat.Size = new Size(194, 31);
            buttonCaiDat.TabIndex = 5;
            buttonCaiDat.Text = "Cài đặt";
            buttonCaiDat.TextAlign = ContentAlignment.MiddleLeft;
            buttonCaiDat.UseVisualStyleBackColor = true;
            buttonCaiDat.Click += buttonCaiDat_Click_1;
            // 
            // pictureBox8
            // 
            pictureBox8.Dock = DockStyle.Fill;
            pictureBox8.Image = Properties.Resources.logout1;
            pictureBox8.Location = new Point(3, 43);
            pictureBox8.Name = "pictureBox8";
            pictureBox8.Size = new Size(44, 35);
            pictureBox8.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox8.TabIndex = 1;
            pictureBox8.TabStop = false;
            // 
            // buttonSanPham
            // 
            buttonSanPham.Dock = DockStyle.Top;
            buttonSanPham.FlatAppearance.BorderSize = 0;
            buttonSanPham.FlatStyle = FlatStyle.Flat;
            buttonSanPham.Location = new Point(0, 150);
            buttonSanPham.Name = "buttonSanPham";
            buttonSanPham.Size = new Size(250, 50);
            buttonSanPham.TabIndex = 4;
            buttonSanPham.Text = "Sản phẩm";
            buttonSanPham.TextAlign = ContentAlignment.MiddleLeft;
            buttonSanPham.UseVisualStyleBackColor = true;
            buttonSanPham.Click += buttonSanPham_Click;
            // 
            // buttonHoaDon
            // 
            buttonHoaDon.Dock = DockStyle.Top;
            buttonHoaDon.FlatAppearance.BorderSize = 0;
            buttonHoaDon.FlatStyle = FlatStyle.Flat;
            buttonHoaDon.Location = new Point(0, 100);
            buttonHoaDon.Name = "buttonHoaDon";
            buttonHoaDon.Size = new Size(250, 50);
            buttonHoaDon.TabIndex = 2;
            buttonHoaDon.Text = "Hóa đơn";
            buttonHoaDon.TextAlign = ContentAlignment.MiddleLeft;
            buttonHoaDon.UseVisualStyleBackColor = true;
            // 
            // buttonKhachHang
            // 
            buttonKhachHang.Dock = DockStyle.Top;
            buttonKhachHang.FlatAppearance.BorderSize = 0;
            buttonKhachHang.FlatStyle = FlatStyle.Flat;
            buttonKhachHang.Location = new Point(0, 50);
            buttonKhachHang.Name = "buttonKhachHang";
            buttonKhachHang.Size = new Size(250, 50);
            buttonKhachHang.TabIndex = 1;
            buttonKhachHang.Text = "Khách hàng";
            buttonKhachHang.TextAlign = ContentAlignment.MiddleLeft;
            buttonKhachHang.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Dock = DockStyle.Top;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.ImageAlign = ContentAlignment.MiddleLeft;
            button1.Location = new Point(0, 0);
            button1.Name = "button1";
            button1.Size = new Size(250, 50);
            button1.TabIndex = 0;
            button1.Text = "Trang chủ";
            button1.TextAlign = ContentAlignment.MiddleLeft;
            button1.UseVisualStyleBackColor = true;
            // 
            // panelDesktop
            // 
            panelDesktop.Dock = DockStyle.Fill;
            panelDesktop.Location = new Point(250, 77);
            panelDesktop.Name = "panelDesktop";
            panelDesktop.Size = new Size(812, 596);
            panelDesktop.TabIndex = 3;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1062, 673);
            Controls.Add(panelDesktop);
            Controls.Add(panelMenu);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Main";
            Text = "Main";
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panelMenu.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox8).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Panel panel1;
        private Panel panelMenu;
        private Panel panelDesktop;
        private Button buttonHoaDon;
        private Button buttonKhachHang;
        private Button button1;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox4;
        private PictureBox pictureBox3;
        private PictureBox pictureBox5;
        private PictureBox pictureBox6;
        private Button buttonSanPham;
        private TableLayoutPanel tableLayoutPanel1;
        private Button buttonDangXuat;
        private Button buttonCaiDat;
        private PictureBox pictureBox8;
    }
}