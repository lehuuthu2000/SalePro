using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace helloworld
{
    public partial class ViewsCaiDat : Form
    {
        private SmtpConfig smtpConfig;

        public ViewsCaiDat()
        {
            InitializeComponent();
            smtpConfig = new SmtpConfig();
            LoadSmtpConfig();
        }

        /// <summary>
        /// Load cấu hình SMTP từ file và hiển thị lên form
        /// </summary>
        private void LoadSmtpConfig()
        {
            try
            {
                smtpConfig.Load();

                // Hiển thị cấu hình lên các textbox
                textBoxSMTPHOST.Text = smtpConfig.SmtpHost;
                textBoxSMTPPOST.Text = smtpConfig.SmtpPort.ToString();
                textBoxSMTPEMAIL.Text = smtpConfig.SmtpUsername;
                textBoxSMTPPASSWORD.Text = smtpConfig.SmtpPassword;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi tải cấu hình SMTP: {ex.Message}",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi click nút Lưu cài đặt
        /// </summary>
        private void buttonLuuCaiDat_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate dữ liệu
                if (string.IsNullOrWhiteSpace(textBoxSMTPHOST.Text))
                {
                    MessageBox.Show("Vui lòng nhập SMTP Host.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBoxSMTPHOST.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBoxSMTPPOST.Text))
                {
                    MessageBox.Show("Vui lòng nhập SMTP Port.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBoxSMTPPOST.Focus();
                    return;
                }

                if (!int.TryParse(textBoxSMTPPOST.Text, out int port) || port <= 0 || port > 65535)
                {
                    MessageBox.Show("SMTP Port phải là số từ 1 đến 65535.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBoxSMTPPOST.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBoxSMTPEMAIL.Text))
                {
                    MessageBox.Show("Vui lòng nhập SMTP Email.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBoxSMTPEMAIL.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBoxSMTPPASSWORD.Text))
                {
                    MessageBox.Show("Vui lòng nhập SMTP Password.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBoxSMTPPASSWORD.Focus();
                    return;
                }

                // Lưu cấu hình
                smtpConfig.SmtpHost = textBoxSMTPHOST.Text.Trim();
                smtpConfig.SmtpPort = port;
                smtpConfig.SmtpUsername = textBoxSMTPEMAIL.Text.Trim();
                smtpConfig.SmtpPassword = textBoxSMTPPASSWORD.Text;
                smtpConfig.Save();

                MessageBox.Show(
                    "Cấu hình SMTP đã được lưu thành công!\n\n" +
                    "Lưu ý: Mật khẩu đã được mã hóa an toàn bằng Windows DPAPI.",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi lưu cấu hình SMTP: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
