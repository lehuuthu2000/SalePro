using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace helloworld
{
    public partial class DangNhapViews : Form
    {
        private AuthBLL dangNhapModels; // Đã chuyển sang AuthBLL

        public DangNhapViews()
        {
            InitializeComponent();
            dangNhapModels = new AuthBLL(); // Đã chuyển sang AuthBLL

            // Bật chế độ ẩn mật khẩu
            aloneTextBoxMatKhau.UseSystemPasswordChar = true;

            // Gắn event handler cho button đăng nhập
            cyberButtonDangNhap.Click += CyberButtonDangNhap_Click;

            // Gắn event handler cho Enter key
            aloneTextBoxMatKhau.KeyDown += AloneTextBoxMatKhau_KeyDown;
            aloneTextBoxUsername.KeyDown += AloneTextBoxUsername_KeyDown;

            // Gắn event handler cho link đăng ký
            linkLabelDangky.Click += LinkLabelDangky_Click;

            // Gắn event handler cho link quên mật khẩu
            linkLabelQuenMatKhau.Click += LinkLabelQuenMatKhau_Click;
        }

        /// <summary>
        /// Xử lý sự kiện khi click vào link "Tạo tài khoản mới" - Mở form đăng ký
        /// </summary>
        private void LinkLabelDangky_Click(object sender, EventArgs e)
        {
            try
            {
                // Mở form đăng ký
                DangKyViews dangKyForm = new DangKyViews();
                if (dangKyForm.ShowDialog() == DialogResult.OK)
                {
                    // Nếu đăng ký thành công, có thể tự động điền username vào ô đăng nhập
                    // (tùy chọn, có thể bỏ qua)
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi mở form đăng ký: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi click vào link "Quên mật khẩu" - Mở form quên mật khẩu
        /// </summary>
        private void LinkLabelQuenMatKhau_Click(object sender, EventArgs e)
        {
            try
            {
                // Mở form quên mật khẩu
                Views.Auth.QuenMatKhau quenMatKhauForm = new Views.Auth.QuenMatKhau();
                quenMatKhauForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi mở form quên mật khẩu: {ex.Message}",
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
                CyberButtonDangNhap_Click(sender, e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Đăng nhập
        /// </summary>
        private async void CyberButtonDangNhap_Click(object sender, EventArgs e)
        {
            try
            {
                string username = aloneTextBoxUsername.Text.Trim();
                string password = aloneTextBoxMatKhau.Text;

                // Validate
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

                // Hiển thị loading
                this.Cursor = Cursors.WaitCursor;
                cyberButtonDangNhap.Enabled = false;

                // Thực hiện đăng nhập
                var user = await dangNhapModels.LoginAsync(username, password);

                this.Cursor = Cursors.Default;
                cyberButtonDangNhap.Enabled = true;

                if (user != null)
                {
                    // Lưu thông tin user vào session
                    UserSession.SetCurrentUser(user);

                    // Đóng form và trả về DialogResult.OK để Program.cs mở Main
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    // Đăng nhập thất bại
                    MessageBox.Show(
                        "Tên đăng nhập hoặc mật khẩu không đúng.\nVui lòng thử lại.",
                        "Đăng nhập thất bại",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    aloneTextBoxMatKhau.Text = "";
                    aloneTextBoxMatKhau.Focus();
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                cyberButtonDangNhap.Enabled = true;
                MessageBox.Show(
                    $"Lỗi khi đăng nhập: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void cyberButtonDangNhap_Click_1(object sender, EventArgs e)
        {

        }
    }
}
