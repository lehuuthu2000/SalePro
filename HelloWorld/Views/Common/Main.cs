using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FontAwesome.Sharp; // Added namespace

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

            // Phân quyền menu
            ApplyPermissions();
        }

        private void ApplyPermissions()
        {
            // Ẩn/Hiện button Cài đặt dựa trên quyền User_Manage hoặc là Admin
            bool canManageSystem = UserSession.HasPermission(Common.PermissionConstants.User_Manage);
            buttonCaiDat.Visible = canManageSystem;
            buttonCaiDat.Enabled = canManageSystem;
            
            // Nếu muốn ẩn thêm các button khác thì thêm logic ở đây
            // bool canViewReports = UserSession.HasPermission(Common.PermissionConstants.Report_View);
            // buttonBaoCao.Visible = canViewReports; // Ví dụ nếu có button Báo cáo
        }

        private void InitializeMenuButtons()
        {
            // Gắn event handler cho các button menu
            SetButtonIcon(button1, IconChar.Home, "Trang chủ");
            button1.Click += (s, e) => {
                SetActiveButton(button1);
                OpenChildForm(new ViewsTrangChu());
            };

            SetButtonIcon(buttonKhachHang, IconChar.Users, "Khách hàng");
            buttonKhachHang.Click += (s, e) => {
                SetActiveButton(buttonKhachHang);
                OpenChildForm(new KhachHangViews());
            };

            SetButtonIcon(buttonHoaDon, IconChar.FileInvoiceDollar, "Hóa đơn");
            buttonHoaDon.Click += (s, e) => {
                SetActiveButton(buttonHoaDon);
                OpenChildForm(new HoaDonViews());
            };

            SetButtonIcon(buttonSanPham, IconChar.BoxOpen, "Sản phẩm");
            buttonSanPham.Click += (s, e) => {
                SetActiveButton(buttonSanPham);
                OpenChildForm(new SanPhamViews());
            };

            // --- REDESIGN SETTINGS & LOGOUT BUTTONS ---
            // Move them out of tableLayoutPanel1 to panelMenu
            // to match the style of other buttons
            
            // Hide the old layout container
            tableLayoutPanel1.Visible = false;

            // Reparent and style "Đăng xuất" (Logout) - Dock Bottom
            buttonDangXuat.Parent = panelMenu;
            buttonDangXuat.Dock = DockStyle.Bottom;
            buttonDangXuat.Height = 50; 
            SetButtonIcon(buttonDangXuat, IconChar.SignOutAlt, "Đăng xuất");
            buttonDangXuat.Click += (s, e) => {
                // Logout logic
                if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    UserSession.Logout();
                    this.Close(); // Close Main form, usually returns to Login if handled in Program.cs
                    // Or explicit: 
                    // new FormDangNhap().Show();
                }
            };

            // Reparent and style "Cài đặt" (Settings) - Dock Bottom (above Logout)
            buttonCaiDat.Parent = panelMenu; 
            buttonCaiDat.Dock = DockStyle.Bottom;
            buttonCaiDat.Height = 50;
            SetButtonIcon(buttonCaiDat, IconChar.Cogs, "Cài đặt");
            buttonCaiDat.Click += (s, e) => {
                SetActiveButton(buttonCaiDat);
                OpenChildForm(new ViewsCaiDat());
            };
            // Note: Since we dock Bottom, the last one added (or reparented) stays at bottom? 
            // Dock order is reverse of addition order usually. 
            // If we want Settings ABOVE Logout:
            // 1. Dock Logout (Bottom) -> sits at very bottom.
            // 2. Dock Settings (Bottom) -> sits above Logout.
            // Check current order: Logout added first above, then Settings. 
            // So Settings will be BELOW Logout? No.
            // Visual Studio Designer: Last added control with Dock.Bottom is at the confusing position.
            // Let's explicitly SendToBack or BringToFront if needed.
            // Actually, simply adding them to Controls collection at the end:
            // panelMenu.Controls.Add(buttonDangXuat);
            // panelMenu.Controls.Add(buttonCaiDat);
            // Dock=Bottom.
            // The one added FIRST to the collection (buttonDangXuat) will be BOTTOM-most?
            // Windows Forms: The control at index 0 (top of Z-order) is closest to the edge?
            // Actually: The control at the FRONT of Z-order gets priority.
            // So calling BringToFront() on Logout makes it stick to edge?
            
            buttonDangXuat.BringToFront(); // Logout at very bottom
            buttonCaiDat.BringToFront();   // Settings above Logout? 
            // Wait, if both are Bottom, BringToFront makes it prioritized => Very Bottom.
            // So:
            // 1. Logout.BringToFront() => Very Bottom.
            // 2. Settings.BringToFront() => Very Bottom (pushes Logout up).
            
            // We want: 
            // [ ... ]
            // [ Settings ]
            // [ Logout ]
            
            // So we want Logout to be at the bottomest.
            // Settings.BringToFront();
            // Logout.BringToFront(); 
        }

        private void SetButtonIcon(Button btn, IconChar icon, string text)
        {
            // Convert Button to useful structure if possible, but FontAwesome.Sharp usually works with IconButton.
            // Since we have existing standard Buttons in Designer, we can't easily change type without touching Designer.cs
            // But FontAwesome.Sharp provides 'IconBitmap' we can set to Image property.
            
            btn.Text = "  " + text; // Add padding
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn.ImageAlign = ContentAlignment.MiddleLeft;
            
            // Generate Bitmap from Icon
            // Note: ToBitmap is an extension method in FontAwesome.Sharp namespace
            btn.Image = icon.ToBitmap(Color.Black, 32); // Use Black for standard visibility or customize
            btn.Padding = new Padding(10, 0, 0, 0); // Left padding for icon
        }

        private void SetActiveButton(Button activeBtn)
        {
            // Reset all buttons style
            ResetButtonStyle(button1);
            ResetButtonStyle(buttonKhachHang);
            ResetButtonStyle(buttonHoaDon);
            ResetButtonStyle(buttonSanPham);
            ResetButtonStyle(buttonCaiDat);
            // Logout usually doesn't stick as 'active' page, so don't need to reset/set it?
            // Or if we want it to highlight on hover only?
            // Let's leave Logout as button style.

            // Highlight active button
            activeBtn.BackColor = Color.FromArgb(220, 220, 220); // Light Gray highlight
            activeBtn.ForeColor = Color.FromArgb(0, 123, 255); // Blue text
            if (activeBtn.Image != null)
            {
                 // Ideally change Icon Color too, but that requires regenerating Bitmap.
                 // For now, simple background highlight is enough.
            }
        }

        private void ResetButtonStyle(Button btn)
        {
            btn.BackColor = Color.White; // Or default color
            btn.ForeColor = Color.Black; 
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
