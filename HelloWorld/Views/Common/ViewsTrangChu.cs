using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.WinForms;
using LiveCharts.Wpf;

using helloworld.Services;

namespace helloworld
{
    public partial class ViewsTrangChu : Form
    {
        private OrderBLL hoaDonModels; // Đã chuyển sang OrderBLL
        private CustomerBLL khachHangModels; // Đã chuyển sang CustomerBLL
        private ProductBLL sanPhamModels;
        private List<Control> dynamicOrderControls = new List<Control>(); // Lưu các controls được tạo động

        public ViewsTrangChu()
        {
            InitializeComponent();
            hoaDonModels = new OrderBLL(); 
            khachHangModels = new CustomerBLL(); 
            sanPhamModels = new ProductBLL();

            InitializeQuickActions();

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
                // Kiểm tra quyền xem báo cáo
                if (!UserSession.HasPermission(Common.PermissionConstants.Report_View))
                {
                    labelDoanhSo.Text = "---";
                    // Ẩn chart nếu có
                    Control? oldChart = tableLayoutPanel14.GetControlFromPosition(0, 1);
                    if (oldChart != null) oldChart.Visible = false;
                }
                else
                {
                    // Tính tổng doanh số từ các hóa đơn đã thanh toán
                    decimal totalRevenue = await hoaDonModels.GetTotalRevenueAsync();
                    labelDoanhSo.Text = totalRevenue.ToString("N0");
                    
                    // Load biểu đồ
                    await LoadChartAsync();
                }

                // Đếm tổng số lượng khách hàng (Quyền xem khách hàng? Tạm thời để chung hoặc không check)
                int totalCustomers = await khachHangModels.GetTotalCustomerCountAsync();
                labelSoLuongKhachHang.Text = totalCustomers.ToString("N0");
                
                // Load hóa đơn gần nhất (Quyền xem hóa đơn - Order_View đã check ở DAL/BLL nhưng ở đây view dashboard)
                if (UserSession.HasPermission(Common.PermissionConstants.Order_View_All))
                {
                   await LoadRecentOrdersAsync();
                }
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

        private async Task LoadChartAsync()
        {
            try
            {
                // Clear old chart if exists in row 1
                // Clear ALL old charts in row 1
                for (int i = tableLayoutPanel14.Controls.Count - 1; i >= 0; i--)
                {
                    Control c = tableLayoutPanel14.Controls[i];
                    if (tableLayoutPanel14.GetRow(c) == 1)
                    {
                        tableLayoutPanel14.Controls.RemoveAt(i);
                        c.Dispose();
                    }
                }

                DataTable dt = await hoaDonModels.GetRevenueLast7DaysAsync();
                
                LiveCharts.WinForms.CartesianChart chart = new LiveCharts.WinForms.CartesianChart();
                chart.Dock = DockStyle.Fill;
                chart.BackColor = Color.White;
                
                ChartValues<decimal> values = new ChartValues<decimal>();
                List<string> labels = new List<string>();
                
                var dataDict = new Dictionary<DateTime, decimal>();
                foreach(DataRow row in dt.Rows) 
                {
                    if (row["date"] != DBNull.Value && row["total"] != DBNull.Value)
                    {
                        dataDict[Convert.ToDateTime(row["date"]).Date] = Convert.ToDecimal(row["total"]);
                    }
                }

                for (int i = 6; i >= 0; i--)
                {
                    DateTime date = DateTime.Today.AddDays(-i);
                    labels.Add(date.ToString("dd/MM"));
                    decimal val = dataDict.ContainsKey(date) ? dataDict[date] : 0;
                    values.Add(val);
                }

                chart.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Doanh thu",
                        Values = values,
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 10,
                        LineSmoothness = 0 // Straight lines
                    }
                };
                
                chart.AxisX.Add(new Axis
                {
                    Labels = labels,
                    Separator = new Separator { Step = 1 } 
                });

                chart.AxisY.Add(new Axis
                {
                    LabelFormatter = value => value.ToString("N0")
                });
                
                chart.LegendLocation = LegendLocation.None;

                tableLayoutPanel14.Controls.Add(chart, 0, 1);
            }
            catch (Exception ex)
            {
                // Log silently or show error
                // MessageBox.Show($"Lỗi load chart: {ex.Message}");
            }
        }

        /// <summary>
        /// Load và hiển thị các hóa đơn gần nhất
        /// </summary>
        private async Task LoadRecentOrdersAsync()
        {
            try
            {
                // Lấy danh sách hóa đơn gần nhất (tối đa 20 hóa đơn)
                List<OrderInfo> recentOrders = await hoaDonModels.GetRecentOrdersAsync(20);

                // Set AutoSize để panel tự giãn khi thêm rows -> kích hoạt Scroll của materialCard4
                tableLayoutPanelHoatDongGanDay.AutoSize = true;
                tableLayoutPanelHoatDongGanDay.AutoSizeMode = AutoSizeMode.GrowAndShrink;

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

        private void InitializeQuickActions()
        {
            // Panel container
            FlowLayoutPanel actionsPanel = new FlowLayoutPanel();
            actionsPanel.Height = 60;
            actionsPanel.Dock = DockStyle.Top;
            actionsPanel.Padding = new Padding(10);
            actionsPanel.BackColor = Color.White;
            
            // Button Import Product
            if (UserSession.HasPermission(Common.PermissionConstants.Product_Add))
            {
                Button btnImport = new Button();
                btnImport.Text = "Nhập SP từ Excel";
                btnImport.AutoSize = true;
                btnImport.Height = 40;
                btnImport.FlatStyle = FlatStyle.Flat;
                btnImport.BackColor = Color.FromArgb(0, 123, 255);
                btnImport.ForeColor = Color.White;
                btnImport.Click += BtnImportProducts_Click;
                btnImport.Margin = new Padding(5);
                actionsPanel.Controls.Add(btnImport);
            }
            
            // Button Export Orders
            if (UserSession.HasPermission(Common.PermissionConstants.Order_View_All))
            {
                Button btnExport = new Button();
                btnExport.Text = "Xuất Hóa Đơn";
                btnExport.AutoSize = true;
                btnExport.Height = 40;
                btnExport.FlatStyle = FlatStyle.Flat;
                btnExport.BackColor = Color.FromArgb(40, 167, 69); // Green
                btnExport.ForeColor = Color.White;
                btnExport.Click += BtnExportOrders_Click;
                btnExport.Margin = new Padding(5);
                actionsPanel.Controls.Add(btnExport);
            }
            
            this.Controls.Add(actionsPanel);
            
            // TLP1 (Index 0) -> Top.
            // ActionsPanel (Index 1) -> Below TLP1.
            this.Controls.SetChildIndex(actionsPanel, 1);
        }

        private async void BtnImportProducts_Click(object sender, EventArgs e)
        {
            try
            {
                ExcelService excel = new ExcelService();
                string? path = excel.ShowOpenFileDialog();
                if (!string.IsNullOrEmpty(path))
                {
                    DataTable dt = excel.ImportFromExcel(path);
                    var result = await sanPhamModels.ImportProductsFromExcelAsync(dt);
                    MessageBox.Show($"Import hoàn tất!\nThành công: {result.success}\nThất bại: {result.failed}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnExportOrders_Click(object sender, EventArgs e)
        {
            try
            {
                ExcelService excel = new ExcelService();
                string? path = excel.ShowSaveFileDialog("Orders.xlsx");
                if (!string.IsNullOrEmpty(path))
                {
                    DataTable dt = await hoaDonModels.LoadOrdersAsync();
                    excel.ExportToExcel(dt, "Orders", path);
                    MessageBox.Show("Xuất file thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
