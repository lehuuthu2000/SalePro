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
        private helloworld.BLL.WooCommerceConfig wcConfig;

        // Controls for WooCommerce
        private TextBox txtWcDomain;
        private TextBox txtWcKey;
        private TextBox txtWcSecret;
        private Button btnTestWc;

        public ViewsCaiDat()
        {
            InitializeComponent();
            smtpConfig = new SmtpConfig();
            wcConfig = new helloworld.BLL.WooCommerceConfig();
            
            InitializeWooCommerceControls();
            
            LoadSmtpConfig();
            LoadWcConfig();
        }

        private void InitializeWooCommerceControls()
        {
            // Create a new GroupBox for WooCommerce
            GroupBox grpWc = new GroupBox();
            grpWc.Text = "WooCommerce";
            grpWc.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            grpWc.Location = new Point(410, 43); // Placement to the right of SMTP or Adjust as needed
            grpWc.Size = new Size(380, 250);
            
            // Adjust layout if needed. The existing form uses TableLayoutPanel.
            // Let's try to add it to the TableLayoutPanel1 if possible, or just add to Controls on top.
            // tableLayoutPanel1 has 2 columns. Column 0 has SMTP. Column 1 is empty?
            // Checking Designer: tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0); 
            // It seems Column 1 is empty. Perfect place for WooCommerce.
            
            Panel panelWc = new Panel();
            panelWc.Dock = DockStyle.Fill;
            
            Label lblTitle = new Label { Text = "WooCommerce", Font = new Font("Segoe UI", 13.8F, FontStyle.Bold), AutoSize = true, Location = new Point(3, 0) };
            panelWc.Controls.Add(lblTitle);
            
            TableLayoutPanel tblWc = new TableLayoutPanel();
            tblWc.Location = new Point(3, 43);
            tblWc.Size = new Size(380, 180);
            tblWc.ColumnCount = 2;
            tblWc.RowCount = 4;
            tblWc.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            tblWc.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            
            // Domain
            tblWc.Controls.Add(new Label { Text = "Domain:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            txtWcDomain = new TextBox { Dock = DockStyle.Fill };
            tblWc.Controls.Add(txtWcDomain, 1, 0);
            
            // Key
            tblWc.Controls.Add(new Label { Text = "Consumer Key:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            txtWcKey = new TextBox { Dock = DockStyle.Fill };
            tblWc.Controls.Add(txtWcKey, 1, 1);
            
            // Secret
            tblWc.Controls.Add(new Label { Text = "Consumer Secret:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
            txtWcSecret = new TextBox { Dock = DockStyle.Fill, PasswordChar = '*' };
            tblWc.Controls.Add(txtWcSecret, 1, 2);
            
            // Test Button
            btnTestWc = new Button { Text = "Kiểm tra kết nối", AutoSize = true };
            btnTestWc.Click += BtnTestWc_Click;
            tblWc.Controls.Add(btnTestWc, 1, 3);
            
            panelWc.Controls.Add(tblWc);
            
            // Add to Main TableLayout (Column 1)
            this.tableLayoutPanel1.Controls.Add(panelWc, 1, 0);
        }

        private void LoadSmtpConfig()
        {
            try
            {
                smtpConfig.Load();
                textBoxSMTPHOST.Text = smtpConfig.SmtpHost;
                textBoxSMTPPOST.Text = smtpConfig.SmtpPort.ToString();
                textBoxSMTPEMAIL.Text = smtpConfig.SmtpUsername;
                textBoxSMTPPASSWORD.Text = smtpConfig.SmtpPassword;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải SMTP: {ex.Message}");
            }
        }

        private void LoadWcConfig()
        {
            try
            {
                wcConfig.Load();
                txtWcDomain.Text = wcConfig.Domain;
                txtWcKey.Text = wcConfig.ConsumerKey;
                txtWcSecret.Text = wcConfig.ConsumerSecret;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải WooCommerce: {ex.Message}");
            }
        }
        
        private async void BtnTestWc_Click(object sender, EventArgs e)
        {
            var service = new helloworld.Services.WooCommerceService();
            try
            {
                bool ok = await service.CheckConnectionAsync(txtWcDomain.Text, txtWcKey.Text, txtWcSecret.Text);
                if (ok) MessageBox.Show("Kết nối WooCommerce thành công!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else MessageBox.Show("Kết nối thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private void buttonLuuCaiDat_Click(object sender, EventArgs e)
        {
            try
            {
                // Save SMTP
                int port = 0;
                int.TryParse(textBoxSMTPPOST.Text, out port);
                
                smtpConfig.SmtpHost = textBoxSMTPHOST.Text.Trim();
                smtpConfig.SmtpPort = port;
                smtpConfig.SmtpUsername = textBoxSMTPEMAIL.Text.Trim();
                smtpConfig.SmtpPassword = textBoxSMTPPASSWORD.Text;
                smtpConfig.Save();
                
                // Save WC
                wcConfig.Domain = txtWcDomain.Text.Trim();
                wcConfig.ConsumerKey = txtWcKey.Text.Trim();
                wcConfig.ConsumerSecret = txtWcSecret.Text.Trim();
                wcConfig.Save();

                MessageBox.Show("Đã lưu tất cả cài đặt thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
