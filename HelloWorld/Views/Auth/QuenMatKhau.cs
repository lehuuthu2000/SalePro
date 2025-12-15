using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using helloworld;

namespace helloworld.Views.Auth
{
    public partial class QuenMatKhau : Form
    {
        private AuthBLL authBLL;

        public QuenMatKhau()
        {
            InitializeComponent();
            authBLL = new AuthBLL();

            // Gắn event handler cho Enter key trong textbox
            aloneTextBoxUsername.KeyDown += AloneTextBoxUsername_KeyDown;
            aloneTextBoxEmail.KeyDown += AloneTextBoxEmail_KeyDown;

            // Focus vào textbox username khi form load
            this.Load += (s, e) => aloneTextBoxUsername.Focus();
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn Enter trong ô username
        /// </summary>
        private void AloneTextBoxUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                aloneTextBoxEmail.Focus();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn Enter trong ô email
        /// </summary>
        private void AloneTextBoxEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cyberButtonGuiMatKhau_Click(sender, e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút "Gửi mật khẩu"
        /// </summary>
        private async void cyberButtonGuiMatKhau_Click(object sender, EventArgs e)
        {
            try
            {
                string username = aloneTextBoxUsername.Text.Trim();
                string email = aloneTextBoxEmail.Text.Trim();

                // Validate - ít nhất một trong hai trường phải có giá trị
                if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show(
                        "Vui lòng nhập tên đăng nhập hoặc địa chỉ email.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    aloneTextBoxUsername.Focus();
                    return;
                }

                // Hiển thị loading
                this.Cursor = Cursors.WaitCursor;
                cyberButtonGuiMatKhau.Enabled = false;
                aloneTextBoxUsername.Enabled = false;
                aloneTextBoxEmail.Enabled = false;

                // Thực hiện gửi yêu cầu cấp lại mật khẩu
                await authBLL.ForgotPasswordAsync(username, email);

                this.Cursor = Cursors.Default;
                cyberButtonGuiMatKhau.Enabled = true;
                aloneTextBoxUsername.Enabled = true;
                aloneTextBoxEmail.Enabled = true;

                // Hiển thị thông báo thành công
                MessageBox.Show(
                    "Yêu cầu cấp lại mật khẩu đã được gửi thành công!\n\n" +
                    "Vui lòng kiểm tra hộp thư email của bạn để nhận mật khẩu mới.\n\n" +
                    "Lưu ý: Mật khẩu mới sẽ được gửi đến địa chỉ email bạn đã đăng ký.\n" +
                    "Nếu không thấy email, vui lòng kiểm tra thư mục Spam/Junk.",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Đóng form sau khi thành công
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (ArgumentException ex)
            {
                // Lỗi validation
                this.Cursor = Cursors.Default;
                cyberButtonGuiMatKhau.Enabled = true;
                aloneTextBoxUsername.Enabled = true;
                aloneTextBoxEmail.Enabled = true;

                MessageBox.Show(
                    ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                
                if (string.IsNullOrWhiteSpace(aloneTextBoxUsername.Text.Trim()))
                {
                    aloneTextBoxUsername.Focus();
                }
                else
                {
                    aloneTextBoxEmail.Focus();
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                cyberButtonGuiMatKhau.Enabled = true;
                aloneTextBoxUsername.Enabled = true;
                aloneTextBoxEmail.Enabled = true;

                MessageBox.Show(
                    $"Lỗi khi gửi yêu cầu: {ex.Message}\n\n" +
                    "Vui lòng kiểm tra:\n" +
                    "- Tên đăng nhập hoặc email đã đăng ký trong hệ thống\n" +
                    "- Tài khoản đã có địa chỉ email\n" +
                    "- Kết nối internet\n" +
                    "- Cấu hình SMTP server",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                
                if (string.IsNullOrWhiteSpace(aloneTextBoxUsername.Text.Trim()))
                {
                    aloneTextBoxUsername.Focus();
                }
                else
                {
                    aloneTextBoxEmail.Focus();
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi click vào link "Tạo tài khoản mới"
        /// </summary>
        private void linkLabelDangky_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                // Mở form đăng ký
                DangKyViews dangKyForm = new DangKyViews();
                if (dangKyForm.ShowDialog() == DialogResult.OK)
                {
                    // Nếu đăng ký thành công, có thể tự động điền thông tin vào form quên mật khẩu
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
        /// Xử lý sự kiện khi click vào link "Đăng nhập tài khoản"
        /// </summary>
        private void linkLabelDangNhapTaiKhoan_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                // Đóng form quên mật khẩu và quay lại form đăng nhập
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
    }
}
