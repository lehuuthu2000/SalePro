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
    public partial class ViewsTrangChu : Form
    {
        private OrderBLL hoaDonModels; // Đã chuyển sang OrderBLL
        private CustomerBLL khachHangModels; // Đã chuyển sang CustomerBLL
        private List<Control> dynamicOrderControls = new List<Control>(); // Lưu các controls được tạo động

        public ViewsTrangChu()
        {
            InitializeComponent();
            hoaDonModels = new OrderBLL(); // Đã chuyển sang OrderBLL
            khachHangModels = new CustomerBLL(); // Đã chuyển sang CustomerBLL

            this.Load += ViewsTrangChu_Load;
            this.VisibleChanged += ViewsTrangChu_VisibleChanged;
        }

        /// <summary>
        /// Load dữ liệu khi form được mở
        /// </summary>
        private async void ViewsTrangChu_Load(object sender, EventArgs e)
        {
            await LoadDashboardDataAsync();
        }

        /// <summary>
        /// Refresh dữ liệu khi form được hiển thị lại (khi quay lại từ form khác)
        /// </summary>
        private async void ViewsTrangChu_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                await LoadDashboardDataAsync();
            }
        }

        /// <summary>
        /// Load và hiển thị dữ liệu dashboard (doanh số và số lượng khách hàng)
        /// </summary>
        private async Task LoadDashboardDataAsync()
        {
            try
            {
                // Tính tổng doanh số từ các hóa đơn đã thanh toán
                decimal totalRevenue = await hoaDonModels.GetTotalRevenueAsync();
                labelDoanhSo.Text = totalRevenue.ToString("N0");

                // Đếm tổng số lượng khách hàng
                int totalCustomers = await khachHangModels.GetTotalCustomerCountAsync();
                labelSoLuongKhachHang.Text = totalCustomers.ToString("N0");

                // Load hóa đơn gần nhất
                await LoadRecentOrdersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi tải dữ liệu dashboard: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Load và hiển thị các hóa đơn gần nhất
        /// </summary>
        private async Task LoadRecentOrdersAsync()
        {
            try
            {
                // Lấy danh sách hóa đơn gần nhất (tối đa 5 hóa đơn)
                List<OrderInfo> recentOrders = await hoaDonModels.GetRecentOrdersAsync(5);

                // Xóa các controls động cũ
                foreach (Control control in dynamicOrderControls)
                {
                    if (control != null && !control.IsDisposed)
                    {
                        tableLayoutPanelHoatDongGanDay.Controls.Remove(control);
                        control.Dispose();
                    }
                }
                dynamicOrderControls.Clear();

                // Xóa các row cũ (trừ row đầu tiên là template)
                while (tableLayoutPanelHoatDongGanDay.RowCount > 1)
                {
                    tableLayoutPanelHoatDongGanDay.RowStyles.RemoveAt(tableLayoutPanelHoatDongGanDay.RowCount - 1);
                    tableLayoutPanelHoatDongGanDay.RowCount--;
                }

                // Cập nhật row đầu tiên với dữ liệu hóa đơn đầu tiên (nếu có)
                if (recentOrders.Count > 0)
                {
                    UpdateOrderRow(0, recentOrders[0], tableLayoutPanel16, labelMaHoaDon, aloneButtonTrangThai, labelTenKhachHang, labelNgayCapNhat);
                }

                // Thêm các row mới cho các hóa đơn còn lại
                for (int i = 1; i < recentOrders.Count; i++)
                {
                    AddOrderRow(i, recentOrders[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi tải hóa đơn gần nhất: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Thêm một row mới cho hóa đơn
        /// </summary>
        private void AddOrderRow(int rowIndex, OrderInfo order)
        {
            // Tăng số row
            tableLayoutPanelHoatDongGanDay.RowCount++;
            tableLayoutPanelHoatDongGanDay.RowStyles.Add(new RowStyle(SizeType.Absolute, 67F));

            // Tạo các controls cho row mới
            // Icon và label "Hóa đơn"
            TableLayoutPanel panelIcon = new TableLayoutPanel();
            panelIcon.ColumnCount = 2;
            panelIcon.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            panelIcon.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panelIcon.Dock = DockStyle.Fill;
            panelIcon.Name = $"panelIcon_{rowIndex}";

            PictureBox picIcon = new PictureBox();
            picIcon.Image = Properties.Resources.invoice_2943203;
            picIcon.SizeMode = PictureBoxSizeMode.Zoom;
            picIcon.Dock = DockStyle.Fill;
            picIcon.Name = $"picIcon_{rowIndex}";

            Label lblType = new Label();
            lblType.Anchor = AnchorStyles.None;
            lblType.AutoSize = true;
            lblType.Font = new Font("Segoe UI Light", 9F);
            lblType.Text = "Hóa đơn";
            lblType.Name = $"lblType_{rowIndex}";

            panelIcon.Controls.Add(picIcon, 0, 0);
            panelIcon.Controls.Add(lblType, 1, 0);

            // Mã hóa đơn
            Label lblOrderCode = new Label();
            lblOrderCode.Anchor = AnchorStyles.None;
            lblOrderCode.AutoSize = true;
            lblOrderCode.Font = new Font("Segoe UI Light", 9F);
            lblOrderCode.Text = order.OrderCode;
            lblOrderCode.Name = $"lblOrderCode_{rowIndex}";

            // Trạng thái
            ReaLTaiizor.Controls.AloneButton btnStatus = new ReaLTaiizor.Controls.AloneButton();
            btnStatus.Anchor = AnchorStyles.None;
            btnStatus.BackColor = GetStatusColor(order.Status);
            btnStatus.EnabledCalc = true;
            btnStatus.Font = new Font("Segoe UI", 9F);
            btnStatus.ForeColor = Color.Black;
            btnStatus.Size = new Size(144, 50);
            btnStatus.Text = order.Status;
            btnStatus.Name = $"btnStatus_{rowIndex}";

            // Tên khách hàng
            Label lblCustomer = new Label();
            lblCustomer.Anchor = AnchorStyles.None;
            lblCustomer.AutoSize = true;
            lblCustomer.Font = new Font("Segoe UI Light", 9F);
            lblCustomer.Text = order.CustomerName ?? "Khách vãng lai";
            lblCustomer.Name = $"lblCustomer_{rowIndex}";

            // Ngày cập nhật
            Label lblDate = new Label();
            lblDate.Anchor = AnchorStyles.None;
            lblDate.AutoSize = true;
            lblDate.Font = new Font("Segoe UI Light", 9F);
            lblDate.Text = FormatRelativeTime(order.UpdatedAt);
            lblDate.Name = $"lblDate_{rowIndex}";

            // Thêm controls vào tableLayoutPanel
            tableLayoutPanelHoatDongGanDay.Controls.Add(panelIcon, 0, rowIndex);
            tableLayoutPanelHoatDongGanDay.Controls.Add(lblOrderCode, 1, rowIndex);
            tableLayoutPanelHoatDongGanDay.Controls.Add(btnStatus, 2, rowIndex);
            tableLayoutPanelHoatDongGanDay.Controls.Add(lblCustomer, 3, rowIndex);
            tableLayoutPanelHoatDongGanDay.Controls.Add(lblDate, 4, rowIndex);

            // Lưu vào danh sách controls động để có thể xóa sau
            dynamicOrderControls.Add(panelIcon);
            dynamicOrderControls.Add(picIcon);
            dynamicOrderControls.Add(lblType);
            dynamicOrderControls.Add(lblOrderCode);
            dynamicOrderControls.Add(btnStatus);
            dynamicOrderControls.Add(lblCustomer);
            dynamicOrderControls.Add(lblDate);
        }

        /// <summary>
        /// Cập nhật row với dữ liệu hóa đơn
        /// </summary>
        private void UpdateOrderRow(int rowIndex, OrderInfo order, TableLayoutPanel panelIcon, Label lblOrderCode, ReaLTaiizor.Controls.AloneButton btnStatus, Label lblCustomer, Label lblDate)
        {
            lblOrderCode.Text = order.OrderCode;
            btnStatus.Text = order.Status;
            btnStatus.BackColor = GetStatusColor(order.Status);
            lblCustomer.Text = order.CustomerName ?? "Khách vãng lai";
            lblDate.Text = FormatRelativeTime(order.UpdatedAt);
        }

        /// <summary>
        /// Lấy màu theo trạng thái
        /// </summary>
        private Color GetStatusColor(string status)
        {
            return status switch
            {
                "Completed" => Color.LightGreen,
                "Pending" => Color.Wheat,
                "Cancelled" => Color.LightCoral,
                _ => Color.LightGray
            };
        }

        /// <summary>
        /// Format thời gian tương đối
        /// </summary>
        private string FormatRelativeTime(DateTime dateTime)
        {
            TimeSpan timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Vừa xong";
            else if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} phút trước";
            else if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} giờ trước";
            else if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} ngày trước";
            else if (dateTime.Date == DateTime.Today)
                return "Hôm nay";
            else if (dateTime.Date == DateTime.Today.AddDays(-1))
                return "Hôm qua";
            else
                return dateTime.ToString("dd/MM/yyyy HH:mm");
        }
    }
}
