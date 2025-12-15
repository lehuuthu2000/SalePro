using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace helloworld
{
    public partial class DangKyViews : Form
    {
        private AuthBLL dangKyModels; // Đã chuyển sang AuthBLL

        public DangKyViews()
        {
            InitializeComponent();
            dangKyModels = new AuthBLL(); // Đã chuyển sang AuthBLL
            
            // Gắn event handler cho button đăng ký
            cyberButtonDangKy.Click += CyberButtonDangKy_Click;
            
            // Bật chế độ ẩn mật khẩu
            aloneTextBoxMatKhau.UseSystemPasswordChar = true;
            
            // Gắn event handler cho Enter key
            aloneTextBoxMatKhau.KeyDown += AloneTextBoxMatKhau_KeyDown;
            aloneTextBoxUsername.KeyDown += AloneTextBoxUsername_KeyDown;
            
            // Gắn event handler cho link đăng nhập
            linkLabelDangNhap.Click += LinkLabelDangNhap_Click;
        }
        
        /// <summary>
        /// Xử lý sự kiện khi click vào link "Đăng nhập tài khoản" - Quay lại form đăng nhập
        /// </summary>
        private void LinkLabelDangNhap_Click(object sender, EventArgs e)
        {
            try
            {
                // Đóng form đăng ký và quay lại form đăng nhập
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn Enter trong ô username
        /// </summary>
        private void AloneTextBoxUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                aloneTextBoxMatKhau.Focus();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn Enter trong ô mật khẩu
        /// </summary>
        private void AloneTextBoxMatKhau_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CyberButtonDangKy_Click(sender, e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Đăng ký
        /// </summary>
        private async void CyberButtonDangKy_Click(object sender, EventArgs e)
        {
            try
            {
                string username = aloneTextBoxUsername.Text.Trim();
                string password = aloneTextBoxMatKhau.Text;

                // Validate username
                if (string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show(
                        "Vui lòng nhập tên đăng nhập.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    aloneTextBoxUsername.Focus();
                    return;
                }

                if (username.Length < 3)
                {
                    MessageBox.Show(
                        "Tên đăng nhập phải có ít nhất 3 ký tự.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    aloneTextBoxUsername.Focus();
                    return;
                }

                if (username.Length > 50)
                {
                    MessageBox.Show(
                        "Tên đăng nhập không được vượt quá 50 ký tự.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    aloneTextBoxUsername.Focus();
                    return;
                }

                // Kiểm tra ký tự đặc biệt trong username (chỉ cho phép chữ, số, dấu gạch dưới)
                if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
                {
                    MessageBox.Show(
                        "Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới (_).",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    aloneTextBoxUsername.Focus();
                    return;
                }

                // Validate password
                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show(
                        "Vui lòng nhập mật khẩu.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    aloneTextBoxMatKhau.Focus();
                    return;
                }

                if (password.Length < 6)
                {
                    MessageBox.Show(
                        "Mật khẩu phải có ít nhất 6 ký tự.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    aloneTextBoxMatKhau.Focus();
                    return;
                }

                if (password.Length > 100)
                {
                    MessageBox.Show(
                        "Mật khẩu không được vượt quá 100 ký tự.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    aloneTextBoxMatKhau.Focus();
                    return;
                }

                // Hiển thị loading
                this.Cursor = Cursors.WaitCursor;
                cyberButtonDangKy.Enabled = false;

                // Thực hiện đăng ký
                await dangKyModels.RegisterAsync(username, password);

                this.Cursor = Cursors.Default;
                cyberButtonDangKy.Enabled = true;

                // Đăng ký thành công
                DialogResult result = MessageBox.Show(
                    $"Đăng ký thành công!\n\nTên đăng nhập: {username}\n\nBạn có muốn đăng nhập ngay bây giờ không?",
                    "Thành công",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                // Đóng form đăng ký
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                cyberButtonDangKy.Enabled = true;
                MessageBox.Show(
                    $"Lỗi khi đăng ký: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
