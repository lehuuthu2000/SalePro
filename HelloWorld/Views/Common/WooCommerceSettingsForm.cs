using System;
using System.Drawing;
using System.Windows.Forms;
using helloworld.BLL;
using helloworld.Services;

namespace helloworld.Views.Common
{
    public class WooCommerceSettingsForm : Form
    {
        private TextBox txtDomain;
        private TextBox txtKey;
        private TextBox txtSecret;
        private Button btnSave;
        private Button btnTest;
        private Button btnClose;
        private WooCommerceConfig _config;

        public WooCommerceSettingsForm()
        {
            InitializeComponent();
            _config = new WooCommerceConfig();
            LoadConfig();
        }

        private void InitializeComponent()
        {
            this.Text = "Cấu hình WooCommerce";
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // Fixed size
            this.MaximizeBox = false;

            int labelWidth = 120;
            int inputWidth = 320;
            int startY = 20;

            // Domain
            var lblDomain = new Label { Text = "Domain (URL):", Location = new Point(20, startY + 3), Width = labelWidth };
            txtDomain = new TextBox { Location = new Point(140, startY), Width = inputWidth };
            this.Controls.Add(lblDomain);
            this.Controls.Add(txtDomain);

            startY += 40;
            // Key
            var lblKey = new Label { Text = "Consumer Key:", Location = new Point(20, startY + 3), Width = labelWidth };
            txtKey = new TextBox { Location = new Point(140, startY), Width = inputWidth };
            this.Controls.Add(lblKey);
            this.Controls.Add(txtKey);

            startY += 40;
            // Secret
            var lblSecret = new Label { Text = "Consumer Secret:", Location = new Point(20, startY + 3), Width = labelWidth };
            txtSecret = new TextBox { Location = new Point(140, startY), Width = inputWidth, PasswordChar = '*' };
            this.Controls.Add(lblSecret);
            this.Controls.Add(txtSecret);

            startY += 60;
            // Buttons
            btnTest = new Button { Text = "Kiểm tra kết nối", Location = new Point(140, startY), Width = 120, Height = 30 };
            btnTest.Click += BtnTest_Click;
            this.Controls.Add(btnTest);

            btnSave = new Button { Text = "Lưu cấu hình", Location = new Point(270, startY), Width = 100, Height = 30 };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
            
            btnClose = new Button { Text = "Đóng", Location = new Point(380, startY), Width = 80, Height = 30 };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void LoadConfig()
        {
            _config.Load();
            txtDomain.Text = _config.Domain;
            txtKey.Text = _config.ConsumerKey;
            txtSecret.Text = _config.ConsumerSecret;
        }

        private async void BtnTest_Click(object sender, EventArgs e)
        {
            var service = new WooCommerceService();
            btnTest.Enabled = false;
            try
            {
                bool ok = await service.CheckConnectionAsync(txtDomain.Text, txtKey.Text, txtSecret.Text);
                if (ok) MessageBox.Show("Kết nối thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else MessageBox.Show("Kết nối thất bại. Vui lòng kiểm tra lại thông tin.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnTest.Enabled = true;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            _config.Domain = txtDomain.Text;
            _config.ConsumerKey = txtKey.Text;
            _config.ConsumerSecret = txtSecret.Text;
            try
            {
                _config.Save();
                MessageBox.Show("Lưu cấu hình thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
