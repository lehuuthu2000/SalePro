namespace helloworld
{
    partial class KhachHangForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KhachHangForm));
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            materialCard1 = new ReaLTaiizor.Controls.MaterialCard();
            tableLayoutPanel3 = new TableLayoutPanel();
            pictureBoxImage = new PictureBox();
            tableLayoutPanel5 = new TableLayoutPanel();
            pictureBoxPhone = new PictureBox();
            pictureBoxEmail = new PictureBox();
            materialCard2 = new ReaLTaiizor.Controls.MaterialCard();
            tableLayoutPanel4 = new TableLayoutPanel();
            textBoxDiemTichLuy = new TextBox();
            label5 = new Label();
            textBoxDiaChi = new TextBox();
            label4 = new Label();
            textBoxEmail = new TextBox();
            label3 = new Label();
            textBoxSoDienThoai = new TextBox();
            label2 = new Label();
            label1 = new Label();
            textBoxHoTen = new TextBox();
            buttonLuu = new Button();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            tableLayoutPanel6 = new TableLayoutPanel();
            dataGridViewHoaDonKhachHang = new DataGridView();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            materialCard1.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImage).BeginInit();
            tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPhone).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxEmail).BeginInit();
            materialCard2.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage2.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewHoaDonKhachHang).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Controls.Add(buttonLuu, 0, 2);
            tableLayoutPanel1.Controls.Add(tabControl1, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 300F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.Size = new Size(932, 722);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(materialCard1, 0, 0);
            tableLayoutPanel2.Controls.Add(materialCard2, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel2.Size = new Size(926, 294);
            tableLayoutPanel2.TabIndex = 0;
            tableLayoutPanel2.Paint += tableLayoutPanel2_Paint;
            // 
            // materialCard1
            // 
            materialCard1.BackColor = Color.FromArgb(255, 255, 255);
            materialCard1.Controls.Add(tableLayoutPanel3);
            materialCard1.Depth = 0;
            materialCard1.Dock = DockStyle.Fill;
            materialCard1.ForeColor = Color.FromArgb(222, 0, 0, 0);
            materialCard1.Location = new Point(14, 14);
            materialCard1.Margin = new Padding(14);
            materialCard1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialCard1.Name = "materialCard1";
            materialCard1.Padding = new Padding(14);
            materialCard1.Size = new Size(435, 266);
            materialCard1.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Controls.Add(pictureBoxImage, 0, 0);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel5, 0, 1);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(14, 14);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 5;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel3.Size = new Size(407, 238);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // pictureBoxImage
            // 
            pictureBoxImage.Anchor = AnchorStyles.None;
            pictureBoxImage.Image = Properties.Resources.profile_picture;
            pictureBoxImage.Location = new Point(128, 3);
            pictureBoxImage.Margin = new Padding(100, 3, 100, 3);
            pictureBoxImage.Name = "pictureBoxImage";
            pictureBoxImage.Size = new Size(151, 114);
            pictureBoxImage.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxImage.TabIndex = 0;
            pictureBoxImage.TabStop = false;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 4;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Controls.Add(pictureBoxPhone, 1, 0);
            tableLayoutPanel5.Controls.Add(pictureBoxEmail, 2, 0);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(3, 123);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel5.Size = new Size(401, 52);
            tableLayoutPanel5.TabIndex = 1;
            // 
            // pictureBoxPhone
            // 
            pictureBoxPhone.Image = Properties.Resources.telephone;
            pictureBoxPhone.Location = new Point(153, 3);
            pictureBoxPhone.Name = "pictureBoxPhone";
            pictureBoxPhone.Size = new Size(44, 46);
            pictureBoxPhone.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxPhone.TabIndex = 0;
            pictureBoxPhone.TabStop = false;
            pictureBoxPhone.Click += PictureBoxPhone_Click;
            // 
            // pictureBoxEmail
            // 
            pictureBoxEmail.Dock = DockStyle.Fill;
            pictureBoxEmail.Image = Properties.Resources.email;
            pictureBoxEmail.Location = new Point(203, 3);
            pictureBoxEmail.Name = "pictureBoxEmail";
            pictureBoxEmail.Size = new Size(44, 46);
            pictureBoxEmail.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxEmail.TabIndex = 1;
            pictureBoxEmail.TabStop = false;
            pictureBoxEmail.Click += PictureBoxEmail_Click;
            // 
            // materialCard2
            // 
            materialCard2.BackColor = Color.FromArgb(255, 255, 255);
            materialCard2.Controls.Add(tableLayoutPanel4);
            materialCard2.Depth = 0;
            materialCard2.Dock = DockStyle.Fill;
            materialCard2.ForeColor = Color.FromArgb(222, 0, 0, 0);
            materialCard2.Location = new Point(477, 14);
            materialCard2.Margin = new Padding(14);
            materialCard2.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialCard2.Name = "materialCard2";
            materialCard2.Padding = new Padding(14);
            materialCard2.Size = new Size(435, 266);
            materialCard2.TabIndex = 1;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.Controls.Add(textBoxDiemTichLuy, 1, 4);
            tableLayoutPanel4.Controls.Add(label5, 0, 4);
            tableLayoutPanel4.Controls.Add(textBoxDiaChi, 1, 3);
            tableLayoutPanel4.Controls.Add(label4, 0, 3);
            tableLayoutPanel4.Controls.Add(textBoxEmail, 1, 2);
            tableLayoutPanel4.Controls.Add(label3, 0, 2);
            tableLayoutPanel4.Controls.Add(textBoxSoDienThoai, 1, 1);
            tableLayoutPanel4.Controls.Add(label2, 0, 1);
            tableLayoutPanel4.Controls.Add(label1, 0, 0);
            tableLayoutPanel4.Controls.Add(textBoxHoTen, 1, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(14, 14);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 6;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel4.Size = new Size(407, 238);
            tableLayoutPanel4.TabIndex = 0;
            // 
            // textBoxDiemTichLuy
            // 
            textBoxDiemTichLuy.Dock = DockStyle.Fill;
            textBoxDiemTichLuy.Location = new Point(133, 203);
            textBoxDiemTichLuy.Name = "textBoxDiemTichLuy";
            textBoxDiemTichLuy.Size = new Size(271, 27);
            textBoxDiemTichLuy.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Location = new Point(3, 200);
            label5.Name = "label5";
            label5.Size = new Size(124, 50);
            label5.TabIndex = 8;
            label5.Text = "Điểm tích lũy :";
            // 
            // textBoxDiaChi
            // 
            textBoxDiaChi.Dock = DockStyle.Fill;
            textBoxDiaChi.Location = new Point(133, 153);
            textBoxDiaChi.Name = "textBoxDiaChi";
            textBoxDiaChi.Size = new Size(271, 27);
            textBoxDiaChi.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Location = new Point(3, 150);
            label4.Name = "label4";
            label4.Size = new Size(124, 50);
            label4.TabIndex = 6;
            label4.Text = "Địa chỉ :";
            // 
            // textBoxEmail
            // 
            textBoxEmail.Dock = DockStyle.Fill;
            textBoxEmail.Location = new Point(133, 103);
            textBoxEmail.Name = "textBoxEmail";
            textBoxEmail.Size = new Size(271, 27);
            textBoxEmail.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(3, 100);
            label3.Name = "label3";
            label3.Size = new Size(124, 50);
            label3.TabIndex = 4;
            label3.Text = "Email :";
            // 
            // textBoxSoDienThoai
            // 
            textBoxSoDienThoai.Dock = DockStyle.Fill;
            textBoxSoDienThoai.Location = new Point(133, 53);
            textBoxSoDienThoai.Name = "textBoxSoDienThoai";
            textBoxSoDienThoai.Size = new Size(271, 27);
            textBoxSoDienThoai.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(3, 50);
            label2.Name = "label2";
            label2.Size = new Size(124, 50);
            label2.TabIndex = 2;
            label2.Text = "Số điện thoại :";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(124, 50);
            label1.TabIndex = 0;
            label1.Text = "Họ tên :";
            // 
            // textBoxHoTen
            // 
            textBoxHoTen.Dock = DockStyle.Fill;
            textBoxHoTen.Location = new Point(133, 3);
            textBoxHoTen.Name = "textBoxHoTen";
            textBoxHoTen.Size = new Size(271, 27);
            textBoxHoTen.TabIndex = 1;
            // 
            // buttonLuu
            // 
            buttonLuu.BackColor = Color.SkyBlue;
            buttonLuu.Dock = DockStyle.Right;
            buttonLuu.Location = new Point(828, 675);
            buttonLuu.Margin = new Padding(3, 3, 10, 3);
            buttonLuu.Name = "buttonLuu";
            buttonLuu.Size = new Size(94, 44);
            buttonLuu.TabIndex = 1;
            buttonLuu.Text = "Lưu";
            buttonLuu.UseVisualStyleBackColor = false;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(3, 303);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(926, 366);
            tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(918, 333);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Thông tin chi tiết";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(tableLayoutPanel6);
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(918, 333);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Hóa đơn";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 1;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel6.Controls.Add(dataGridViewHoaDonKhachHang, 0, 1);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(3, 3);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 2;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel6.Size = new Size(912, 327);
            tableLayoutPanel6.TabIndex = 0;
            // 
            // dataGridViewHoaDonKhachHang
            // 
            dataGridViewHoaDonKhachHang.BackgroundColor = SystemColors.Control;
            dataGridViewHoaDonKhachHang.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewHoaDonKhachHang.Dock = DockStyle.Fill;
            dataGridViewHoaDonKhachHang.Location = new Point(3, 43);
            dataGridViewHoaDonKhachHang.Name = "dataGridViewHoaDonKhachHang";
            dataGridViewHoaDonKhachHang.RowHeadersWidth = 51;
            dataGridViewHoaDonKhachHang.Size = new Size(906, 281);
            dataGridViewHoaDonKhachHang.TabIndex = 0;
            // 
            // KhachHangForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(932, 722);
            Controls.Add(tableLayoutPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "KhachHangForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Thêm mới khách hàng";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            materialCard1.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxImage).EndInit();
            tableLayoutPanel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxPhone).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxEmail).EndInit();
            materialCard2.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tableLayoutPanel6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewHoaDonKhachHang).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private ReaLTaiizor.Controls.MaterialCard materialCard1;
        private TableLayoutPanel tableLayoutPanel3;
        private PictureBox pictureBoxImage;
        private ReaLTaiizor.Controls.MaterialCard materialCard2;
        private TableLayoutPanel tableLayoutPanel4;
        private Label label1;
        private TextBox textBoxHoTen;
        private Button buttonLuu;
        private TextBox textBoxDiaChi;
        private Label label4;
        private TextBox textBoxEmail;
        private Label label3;
        private TextBox textBoxSoDienThoai;
        private Label label2;
        private TextBox textBoxDiemTichLuy;
        private Label label5;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TableLayoutPanel tableLayoutPanel5;
        private PictureBox pictureBoxPhone;
        private PictureBox pictureBoxEmail;
        private TableLayoutPanel tableLayoutPanel6;
        private DataGridView dataGridViewHoaDonKhachHang;
    }
}