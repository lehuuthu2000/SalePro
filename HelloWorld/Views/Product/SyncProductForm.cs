using System;
using System.Drawing;
using System.Windows.Forms;
using helloworld.BLL;

namespace helloworld.Views.Product
{
    public class SyncProductForm : Form
    {
        private Button btnSync;
        private ListBox lbLogs;
        private ProgressBar progressBar;
        private WooCommerceBLL _bll;
        private WooCommerceConfig _config;

        public SyncProductForm()
        {
            InitializeComponent();
            _bll = new WooCommerceBLL();
            _config = new WooCommerceConfig();
            _config.Load();
        }

        private void InitializeComponent()
        {
            this.Text = "Đồng bộ sản phẩm từ WooCommerce";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            var lblInfo = new Label 
            { 
                Text = "Nhấn nút 'Bắt đầu đồng bộ' để lấy dữ liệu từ Website về.",
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblInfo);

            btnSync = new Button
            {
                Text = "Bắt đầu đồng bộ",
                Location = new Point(20, 50),
                Width = 150,
                Height = 40,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };
            btnSync.Click += BtnSync_Click;
            this.Controls.Add(btnSync);

            progressBar = new ProgressBar
            {
                Location = new Point(190, 55),
                Width = 370,
                Height = 30,
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };
            this.Controls.Add(progressBar);

            lbLogs = new ListBox
            {
                Location = new Point(20, 110),
                Width = 540,
                Height = 230
            };
            this.Controls.Add(lbLogs);
        }

        private async void BtnSync_Click(object sender, EventArgs e)
        {
            if (!_config.IsConfigured())
            {
                MessageBox.Show("Chưa cấu hình kết nối WooCommerce. Vui lòng vào Cài đặt -> Cấu hình WooCommerce.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnSync.Enabled = false;
            progressBar.Visible = true;
            lbLogs.Items.Clear();
            lbLogs.Items.Add("Bắt đầu đồng bộ...");

            try
            {
                // Action progress handler
                Action<string> onProgress = (msg) => 
                {
                    this.Invoke((MethodInvoker)delegate {
                        lbLogs.Items.Add($"{DateTime.Now:HH:mm:ss} - {msg}");
                        lbLogs.TopIndex = lbLogs.Items.Count - 1;
                    });
                };

                string result = await _bll.SyncProductsAsync(_config.Domain, _config.ConsumerKey, _config.ConsumerSecret, onProgress);
                
                lbLogs.Items.Add("--- KẾT QUẢ ---");
                lbLogs.Items.Add(result);
                MessageBox.Show(result, "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi không mong muốn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lbLogs.Items.Add($"EXCEPTION: {ex.Message}");
            }
            finally
            {
                btnSync.Enabled = true;
                progressBar.Visible = false;
            }
        }
    }
}
