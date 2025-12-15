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
    public partial class KhachHangViews : Form
    {
        private CustomerBLL khachHangModels; // Đã chuyển sang CustomerBLL
        
        // DataBinding cho khách hàng
        private BindingSource bindingSource = new BindingSource();
        private DataTable customersDataTable;
        private DataView customersDataView;

        public KhachHangViews()
        {
            InitializeComponent();
            khachHangModels = new CustomerBLL(); // Đã chuyển sang CustomerBLL
            
            // Khởi tạo BindingSource và gắn vào DataGridView
            dataGridView1.DataSource = bindingSource;
            
            this.Load += ViewsKhachHang_Load;
            buttonThemmoi.Click += ButtonThemmoi_Click;
            buttonXoa.Click += ButtonXoa_Click;
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
            textBoxTimKiem.TextChanged += TextBoxTimKiem_TextChanged;
        }

        private async void ViewsKhachHang_Load(object sender, EventArgs e)
        {
            await LoadCustomersAsync();
        }

        /// <summary>
        /// Load danh sách khách hàng từ bảng Customers và hiển thị vào DataGridView
        /// </summary>
        private async Task LoadCustomersAsync()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                
                // Lấy danh sách khách hàng từ BLL
                customersDataTable = await khachHangModels.LoadCustomersAsync();
                
                // Tạo DataView để hỗ trợ tìm kiếm và sắp xếp
                customersDataView = customersDataTable.DefaultView;
                
                // Bind vào BindingSource
                bindingSource.DataSource = customersDataView;
                
                // Đảm bảo DataGridView được bind đúng
                if (dataGridView1.DataSource != bindingSource)
                {
                    dataGridView1.DataSource = bindingSource;
                }
                
                // Cấu hình DataGridView
                ConfigureDataGridView();
                
                // Refresh DataGridView
                dataGridView1.Refresh();
                
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(
                    $"Lỗi khi tải danh sách khách hàng: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
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
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Thêm mới - Mở form KhachHangForm ở Add mode
        /// </summary>
        private async void ButtonThemmoi_Click(object sender, EventArgs e)
        {
            try
            {
                KhachHangForm addForm = new KhachHangForm(); // Add mode
                addForm.ShowDialog(); // Hiển thị form dạng modal
                
                // Sau khi đóng form thêm mới, reload lại danh sách khách hàng
                await LoadCustomersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi mở form thêm khách hàng: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Xóa - Xóa khách hàng đã chọn
        /// </summary>
        private async void ButtonXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem có dòng nào được chọn không
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show(
                        "Vui lòng chọn ít nhất một khách hàng để xóa.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // Xác nhận xóa
                DialogResult confirmResult = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa {dataGridView1.SelectedRows.Count} khách hàng đã chọn?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmResult != DialogResult.Yes)
                {
                    return;
                }

                // Lấy danh sách ID khách hàng được chọn từ bindingSource
                List<int> customerIds = new List<int>();
                
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    // Lấy DataRowView từ bindingSource
                    DataRowView rowView = (DataRowView)row.DataBoundItem;
                    
                    if (rowView["Mã KH"] != null && 
                        int.TryParse(rowView["Mã KH"].ToString(), out int customerId))
                    {
                        customerIds.Add(customerId);
                    }
                }

                if (customerIds.Count == 0)
                {
                    MessageBox.Show(
                        "Không thể lấy được thông tin khách hàng để xóa.",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                // Disable nút Xóa để tránh click nhiều lần
                buttonXoa.Enabled = false;
                buttonXoa.Text = "Đang xóa...";

                // Thực hiện xóa
                DeleteResult result = await khachHangModels.DeleteCustomersAsync(customerIds);

                // Hiển thị kết quả
                string message = $"Đã xóa thành công {result.SuccessCount} khách hàng.";

                if (result.FailedCustomers.Count > 0)
                {
                    message += $"\n\nKhông thể xóa {result.FailedCustomers.Count} khách hàng:";
                    foreach (var failed in result.FailedCustomers)
                    {
                        message += $"\n- Khách hàng ID {failed.CustomerId}: {failed.Reason}";
                        
                        if (failed.RelatedOrders.Count > 0)
                        {
                            message += "\n  Các hóa đơn liên quan:";
                            foreach (var order in failed.RelatedOrders)
                            {
                                message += $"\n    • {order.OrderCode} - {order.TotalAmount:N0} VNĐ - {order.Status} ({order.CreatedAt:dd/MM/yyyy})";
                            }
                        }
                    }
                }

                MessageBox.Show(
                    message,
                    result.FailedCustomers.Count > 0 ? "Kết quả xóa" : "Thành công",
                    MessageBoxButtons.OK,
                    result.FailedCustomers.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information
                );

                // Reload danh sách khách hàng
                await LoadCustomersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi xóa khách hàng: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                buttonXoa.Enabled = true;
                buttonXoa.Text = "Xóa";
            }
        }

        /// <summary>
        /// Xử lý tìm kiếm real-time khi người dùng gõ vào ô tìm kiếm
        /// </summary>
        private void TextBoxTimKiem_TextChanged(object sender, EventArgs e)
        {
            FilterCustomers(textBoxTimKiem.Text.Trim());
        }

        /// <summary>
        /// Lọc khách hàng theo từ khóa sử dụng DataView.RowFilter (real-time)
        /// </summary>
        private void FilterCustomers(string keyword)
        {
            if (customersDataView == null || customersDataTable == null)
                return;

            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    // Xóa filter để hiển thị tất cả dữ liệu
                    customersDataView.RowFilter = string.Empty;
                }
                else
                {
                    // Escape các ký tự đặc biệt trong DataView RowFilter
                    string escapedKeyword = keyword.Replace("[", "[[]")
                                                   .Replace("]", "[]]")
                                                   .Replace("*", "[*]")
                                                   .Replace("%", "[%]")
                                                   .Replace("'", "''")
                                                   .Replace("#", "[#]");

                    // Lọc dữ liệu theo nhiều cột (text và numeric)
                    string filter = $@"
                        ([Mã KH] LIKE '%{escapedKeyword}%' OR 
                         [Họ tên] LIKE '%{escapedKeyword}%' OR 
                         [Số điện thoại] LIKE '%{escapedKeyword}%' OR 
                         [Email] LIKE '%{escapedKeyword}%' OR 
                         [Địa chỉ] LIKE '%{escapedKeyword}%' OR 
                         CONVERT([Điểm tích lũy], 'System.String') LIKE '%{escapedKeyword}%')";

                    customersDataView.RowFilter = filter;
                }
            }
            catch (Exception ex)
            {
                // Nếu có lỗi trong filter, xóa filter và hiển thị tất cả
                customersDataView.RowFilter = string.Empty;
                MessageBox.Show(
                    $"Lỗi khi tìm kiếm: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi double click vào dòng trong DataGridView - Mở form Edit
        /// </summary>
        private async void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Kiểm tra xem có click vào dòng hợp lệ không
                if (e.RowIndex < 0)
                {
                    return;
                }

                // Lấy customer_id từ bindingSource
                DataRowView rowView = (DataRowView)dataGridView1.Rows[e.RowIndex].DataBoundItem;
                
                if (rowView["Mã KH"] != null && 
                    int.TryParse(rowView["Mã KH"].ToString(), out int customerId))
                {
                    // Mở form Edit với customerId
                    KhachHangForm editForm = new KhachHangForm(customerId);
                    editForm.ShowDialog();
                    
                    // Sau khi đóng form edit, reload lại danh sách khách hàng
                    if (editForm.DialogResult == DialogResult.OK)
                    {
                        await LoadCustomersAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi mở form chỉnh sửa khách hàng: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
