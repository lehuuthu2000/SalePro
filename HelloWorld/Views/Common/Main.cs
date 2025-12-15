using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace helloworld
{
    public partial class Main : Form
    {
        private Form currentChildForm;

        public Main()
        {
            InitializeComponent();
            InitializeMenuButtons();
            this.Load += Main_Load;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // Tự động hiển thị Trang chủ khi form được mở
            OpenChildForm(new ViewsTrangChu());
        }

        private void InitializeMenuButtons()
        {
            // Gắn event handler cho các button menu
            button1.Click += ButtonTrangChu_Click; // Trang chủ
            buttonKhachHang.Click += ButtonKhachHang_Click; // Khách hàng
            buttonHoaDon.Click += ButtonHoaDon_Click; // Hóa đơn
            //button3.Click += ButtonCaiDat_Click; // Cài đặt
        }

        /// <summary>
        /// Xử lý sự kiện khi click vào button Trang chủ
        /// </summary>
        private void ButtonTrangChu_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ViewsTrangChu());
        }

        /// <summary>
        /// Xử lý sự kiện khi click vào button Khách hàng
        /// </summary>
        private void ButtonKhachHang_Click(object sender, EventArgs e)
        {
            OpenChildForm(new KhachHangViews());
        }

        /// <summary>
        /// Xử lý sự kiện khi click vào button Hóa đơn
        /// </summary>
        private void ButtonHoaDon_Click(object sender, EventArgs e)
        {
            OpenChildForm(new HoaDonViews());
        }

        /// <summary>
        /// Xử lý sự kiện khi click vào button Cài đặt
        /// </summary>
        private void ButtonCaiDat_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ViewsCaiDat());
        }

        /// <summary>
        /// Mở form con trong panelDesktop
        /// </summary>
        /// <param name="childForm">Form cần hiển thị</param>
        private void OpenChildForm(Form childForm)
        {
            // Đóng form hiện tại nếu có
            if (currentChildForm != null)
            {
                currentChildForm.Close();
            }

            // Cấu hình form con để hiển thị trong panel
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            // Xóa tất cả controls cũ trong panelDesktop
            panelDesktop.Controls.Clear();

            // Thêm form con vào panelDesktop
            panelDesktop.Controls.Add(childForm);
            panelDesktop.Tag = childForm;

            // Hiển thị form
            childForm.BringToFront();
            childForm.Show();

            // Lưu reference để có thể đóng sau này
            currentChildForm = childForm;
        }

        private void buttonSanPham_Click(object sender, EventArgs e)
        {
            OpenChildForm(new SanPhamViews());
        }

        private void buttonCaiDat_Click_1(object sender, EventArgs e)
        {
            OpenChildForm(new ViewsCaiDat());
        }
    }
}
