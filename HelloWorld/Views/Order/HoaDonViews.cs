using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace helloworld
{
    public partial class HoaDonViews : Form
    {
        private OrderBLL hoaDonModels; // Đã chuyển sang OrderBLL
        private BindingSource bindingSource = new BindingSource(); // BindingSource cho Orders
        private DataTable ordersDataTable; // Lưu DataTable để filter
        private DataView ordersDataView; // DataView để filter nhanh

        public HoaDonViews()
        {
            InitializeComponent();
            hoaDonModels = new OrderBLL(); // Đã chuyển sang OrderBLL
            
            // Khởi tạo BindingSource và gắn vào DataGridView
            dataGridView1.DataSource = bindingSource;
            
            this.Load += HoaDonViews_Load;
            buttonThemmoi.Click += ButtonThemmoi_Click;
            buttonXoa.Click += ButtonXoa_Click;
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
            textBoxTimKiem.TextChanged += TextBoxTimKiem_TextChanged;
        }

        /// <summary>
        /// Load danh sách hóa đơn khi form được mở
        /// </summary>
        private async void HoaDonViews_Load(object sender, EventArgs e)
        {
            await LoadOrdersAsync();
        }

        /// <summary>
        /// Load danh sách hóa đơn từ bảng Orders và hiển thị vào DataGridView
        /// Sử dụng BindingSource và DataView để hỗ trợ filter nhanh
        /// </summary>
        private async Task LoadOrdersAsync()
        {
            try
            {
                // Hiển thị loading indicator
                this.Cursor = Cursors.WaitCursor;
                bindingSource.DataSource = null;
                dataGridView1.Refresh();

                // Gọi method từ BLL để lấy dữ liệu
                ordersDataTable = await hoaDonModels.LoadOrdersAsync();

                // Tạo DataView từ DataTable để hỗ trợ filter
                ordersDataView = ordersDataTable.DefaultView;

                // Bind DataView vào BindingSource
                bindingSource.DataSource = ordersDataView;
                
                // DataGridView đã được bind với bindingSource trong constructor
                // Không cần set lại DataSource

                // Cấu hình DataGridView
                ConfigureDataGridView();
                
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(
                    $"Lỗi khi tải danh sách hóa đơn: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Tìm kiếm hóa đơn real-time - Sử dụng DataView filter thay vì query lại database
        /// </summary>
        private void TextBoxTimKiem_TextChanged(object sender, EventArgs e)
        {
            // Sử dụng DataView filter - nhanh hơn, không cần query lại database
            FilterOrders(textBoxTimKiem.Text.Trim());
        }

        /// <summary>
        /// Filter hóa đơn sử dụng DataView (nhanh, không cần query database)
        /// </summary>
        private void FilterOrders(string keyword)
        {
            try
            {
                if (ordersDataView == null)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    // Xóa filter - hiển thị tất cả
                    ordersDataView.RowFilter = "";
                }
                else
                {
                    // Filter theo nhiều cột
                    // Escape ký tự đặc biệt trong keyword để tránh lỗi SQL injection trong filter
                    string escapedKeyword = keyword.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]");
                    
                    // Kiểm tra xem keyword có phải là số không (để tìm theo Mã HĐ, Mã KH, Mã NV)
                    bool isNumeric = int.TryParse(keyword, out int numericValue);
                    
                    // Filter theo: Mã hóa đơn, Mã HĐ, Mã KH, Mã NV, Trạng thái
                    string filter = $"[Mã hóa đơn] LIKE '%{escapedKeyword}%' OR [Trạng thái] LIKE '%{escapedKeyword}%'";
                    
                    // Nếu là số, thêm filter cho các cột số
                    if (isNumeric)
                    {
                        filter += $" OR [Mã HĐ] = {numericValue} OR [Mã KH] = {numericValue} OR [Mã NV] = {numericValue}";
                    }
                    
                    ordersDataView.RowFilter = $"({filter})";
                }
            }
            catch (Exception ex)
            {
                // Nếu filter có lỗi (ví dụ: cú pháp sai), hiển thị tất cả
                if (ordersDataView != null)
                {
                    ordersDataView.RowFilter = "";
                }
                MessageBox.Show(
                    $"Lỗi khi tìm kiếm: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        /// <summary>
        /// Cấu hình hiển thị cho DataGridView
        /// </summary>
        private void ConfigureDataGridView()
        {
            // Tự động điều chỉnh độ rộng cột
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Cho phép chọn nhiều dòng
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = true;

            // Ẩn cột row header mặc định
            dataGridView1.RowHeadersVisible = false;

            // Cho phép đọc-only (không cho chỉnh sửa trực tiếp)
            dataGridView1.ReadOnly = true;

            // Căn giữa header
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Style cho header
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            // Format cột tiền tệ
            if (dataGridView1.Columns["Tổng tiền"] != null)
            {
                dataGridView1.Columns["Tổng tiền"].DefaultCellStyle.Format = "N0";
                dataGridView1.Columns["Tổng tiền"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dataGridView1.Columns["Giảm giá"] != null)
            {
                dataGridView1.Columns["Giảm giá"].DefaultCellStyle.Format = "N0";
                dataGridView1.Columns["Giảm giá"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            // Căn giữa cho cột ID
            if (dataGridView1.Columns["Mã HĐ"] != null)
            {
                dataGridView1.Columns["Mã HĐ"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dataGridView1.Columns["Mã KH"] != null)
            {
                dataGridView1.Columns["Mã KH"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dataGridView1.Columns["Mã NV"] != null)
            {
                dataGridView1.Columns["Mã NV"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Thêm mới - Mở form HoaDonForm
        /// </summary>
        private async void ButtonThemmoi_Click(object sender, EventArgs e)
        {
            try
            {
                HoaDonForm addForm = new HoaDonForm(); // Add mode
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    // Reload lại danh sách hóa đơn
                    await LoadOrdersAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi mở form thêm hóa đơn: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi double click vào một hóa đơn - Mở form chỉnh sửa
        /// </summary>
        private async void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;

                // Lấy Order ID từ bindingSource thay vì trực tiếp từ DataGridView
                DataRowView selectedRow = (DataRowView)bindingSource[e.RowIndex];
                
                if (selectedRow["Mã HĐ"] == null)
                    return;

                // Lấy Order ID từ cột "Mã HĐ"
                int orderId = Convert.ToInt32(selectedRow["Mã HĐ"]);

                // Mở form chỉnh sửa với Order ID
                HoaDonForm editForm = new HoaDonForm(orderId); // Edit mode
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // Reload lại danh sách hóa đơn
                    await LoadOrdersAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi mở form chỉnh sửa hóa đơn: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Xóa - Xóa hóa đơn đã chọn
        /// </summary>
        private async void ButtonXoa_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có hóa đơn nào được chọn không
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn ít nhất một hóa đơn để xóa.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            // Xác nhận xóa
            int selectedCount = dataGridView1.SelectedRows.Count;
            DialogResult confirmResult = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {selectedCount} hóa đơn đã chọn?\n\n" +
                "Lưu ý: Hành động này không thể hoàn tác!",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            try
            {
                // Lấy danh sách Order ID từ bindingSource
                List<int> orderIds = new List<int>();

                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    // Lấy DataRowView từ bindingSource
                    DataRowView rowView = (DataRowView)row.DataBoundItem;
                    
                    if (rowView["Mã HĐ"] != null &&
                        int.TryParse(rowView["Mã HĐ"].ToString(), out int orderId))
                    {
                        orderIds.Add(orderId);
                    }
                }

                if (orderIds.Count == 0)
                {
                    MessageBox.Show(
                        "Không thể lấy thông tin hóa đơn từ các dòng được chọn.",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                // Hiển thị loading
                this.Cursor = Cursors.WaitCursor;
                buttonXoa.Enabled = false;

                // Thực hiện xóa
                var deleteResult = await hoaDonModels.DeleteOrdersAsync(orderIds);

                this.Cursor = Cursors.Default;
                buttonXoa.Enabled = true;

                // Hiển thị kết quả
                string message = $"Đã xóa thành công {deleteResult.SuccessCount} hóa đơn.";

                if (deleteResult.FailedOrders.Count > 0)
                {
                    message += $"\n\nKhông thể xóa {deleteResult.FailedOrders.Count} hóa đơn:";
                    foreach (var failed in deleteResult.FailedOrders)
                    {
                        message += $"\n- {failed.OrderCode} (Mã HĐ: {failed.OrderId}): {failed.Reason}";
                    }
                }

                MessageBox.Show(
                    message,
                    deleteResult.FailedOrders.Count > 0 ? "Kết quả xóa" : "Thành công",
                    MessageBoxButtons.OK,
                    deleteResult.FailedOrders.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information
                );

                // Reload danh sách hóa đơn
                if (deleteResult.SuccessCount > 0)
                {
                    await LoadOrdersAsync();
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                buttonXoa.Enabled = true;
                MessageBox.Show(
                    $"Lỗi khi xóa hóa đơn: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
