using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace helloworld
{
    public partial class SanPhamViews : Form
    {
        private ProductBLL sanPhamModels; // Đã chuyển sang ProductBLL
        private BindingSource bindingSource = new BindingSource();
        private DataTable productsDataTable;
        private DataView productsDataView;

        public SanPhamViews()
        {
            InitializeComponent();
            sanPhamModels = new ProductBLL();
            
            // Khởi tạo BindingSource và gắn vào DataGridView
            dataGridViewSanPham.DataSource = bindingSource;
            
            this.Load += SanPhamViews_Load;
            buttonThemmoi.Click += ButtonThemmoi_Click;
            buttonXoa.Click += ButtonXoa_Click;
            textBoxTimKiem.TextChanged += TextBoxTimKiem_TextChanged;
            dataGridViewSanPham.CellDoubleClick += DataGridViewSanPham_CellDoubleClick;
            buttonWordpress.Click += ButtonWordpress_Click;
        }

        /// <summary>
        /// Load danh sách sản phẩm khi form được mở
        /// </summary>
        private async void SanPhamViews_Load(object sender, EventArgs e)
        {
            await LoadProductsAsync();
        }

        private void ButtonWordpress_Click(object sender, EventArgs e)
        {
            var form = new helloworld.Views.Product.SyncProductForm();
            form.ShowDialog();
            // Reload after sync?
            _ = LoadProductsAsync();
        }

        /// <summary>
        /// Load danh sách sản phẩm từ database và hiển thị vào DataGridView
        /// Sử dụng BindingSource và DataView để hỗ trợ filter nhanh
        /// </summary>
        private async Task LoadProductsAsync()
        {
            try
            {
                // Hiển thị loading indicator
                this.Cursor = Cursors.WaitCursor;
                bindingSource.DataSource = null;
                dataGridViewSanPham.Refresh();

                // Gọi method từ BLL để lấy dữ liệu
                productsDataTable = await sanPhamModels.LoadProductsAsync();

                // Tạo DataView từ DataTable để hỗ trợ filter
                productsDataView = productsDataTable.DefaultView;

                // Bind DataView vào BindingSource
                bindingSource.DataSource = productsDataView;
                
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
                    $"Lỗi khi tải danh sách sản phẩm: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Tìm kiếm sản phẩm - Sử dụng DataView filter thay vì query lại database
        /// </summary>
        private void TextBoxTimKiem_TextChanged(object sender, EventArgs e)
        {
            // Sử dụng DataView filter - nhanh hơn, không cần query lại database
            FilterProducts(textBoxTimKiem.Text.Trim());
        }

        /// <summary>
        /// Filter sản phẩm sử dụng DataView (nhanh, không cần query database)
        /// </summary>
        private void FilterProducts(string keyword)
        {
            try
            {
                if (productsDataView == null)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    // Xóa filter - hiển thị tất cả
                    productsDataView.RowFilter = "";
                }
                else
                {
                    // Filter theo nhiều cột
                    // Escape ký tự đặc biệt trong keyword để tránh lỗi SQL injection trong filter
                    string escapedKeyword = keyword.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]");
                    
                    productsDataView.RowFilter = $"(product_name LIKE '%{escapedKeyword}%' OR " +
                                                 $"product_code LIKE '%{escapedKeyword}%' OR " +
                                                 $"category_name LIKE '%{escapedKeyword}%')";
                }
            }
            catch (Exception ex)
            {
                // Nếu filter có lỗi (ví dụ: cú pháp sai), hiển thị tất cả
                if (productsDataView != null)
                {
                    productsDataView.RowFilter = "";
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
            dataGridViewSanPham.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Cho phép chọn nhiều dòng
            dataGridViewSanPham.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewSanPham.MultiSelect = true;

            // Ẩn cột row header mặc định
            dataGridViewSanPham.RowHeadersVisible = false;

            // Cho phép đọc-only (không cho chỉnh sửa trực tiếp)
            dataGridViewSanPham.ReadOnly = true;

            // Căn giữa header
            dataGridViewSanPham.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Style cho header
            dataGridViewSanPham.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            // Format cột số
            if (dataGridViewSanPham.Columns["Số biến thể"] != null)
            {
                dataGridViewSanPham.Columns["Số biến thể"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dataGridViewSanPham.Columns["Tổng tồn kho"] != null)
            {
                dataGridViewSanPham.Columns["Tổng tồn kho"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridViewSanPham.Columns["Tổng tồn kho"].DefaultCellStyle.Format = "N0";
            }

            if (dataGridViewSanPham.Columns["Tổng số lượng bán"] != null)
            {
                dataGridViewSanPham.Columns["Tổng số lượng bán"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridViewSanPham.Columns["Tổng số lượng bán"].DefaultCellStyle.Format = "N0";
            }

            // Format cột tiền tệ
            if (dataGridViewSanPham.Columns["Giá thấp nhất"] != null)
            {
                dataGridViewSanPham.Columns["Giá thấp nhất"].DefaultCellStyle.Format = "N0";
                dataGridViewSanPham.Columns["Giá thấp nhất"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dataGridViewSanPham.Columns["Giá cao nhất"] != null)
            {
                dataGridViewSanPham.Columns["Giá cao nhất"].DefaultCellStyle.Format = "N0";
                dataGridViewSanPham.Columns["Giá cao nhất"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dataGridViewSanPham.Columns["Tổng số tiền bán"] != null)
            {
                dataGridViewSanPham.Columns["Tổng số tiền bán"].DefaultCellStyle.Format = "N0";
                dataGridViewSanPham.Columns["Tổng số tiền bán"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            // Căn giữa cho cột ID
            if (dataGridViewSanPham.Columns["Mã SP"] != null)
            {
                dataGridViewSanPham.Columns["Mã SP"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Thêm mới - Mở form SanPhamForm
        /// </summary>
        private async void ButtonThemmoi_Click(object sender, EventArgs e)
        {
            try
            {
                SanPhamForm addForm = new SanPhamForm(); // Add mode
                addForm.ShowDialog(); // Hiển thị form dạng modal

                // Sau khi đóng form thêm mới, reload lại danh sách sản phẩm
                await LoadProductsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi mở form thêm sản phẩm: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi double-click vào sản phẩm trong DataGridView - Mở form chỉnh sửa
        /// </summary>
        private async void DataGridViewSanPham_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    // Lấy Product ID từ bindingSource thay vì trực tiếp từ DataGridView
                    DataRowView selectedRow = (DataRowView)bindingSource[e.RowIndex];
                    
                    if (selectedRow["Mã SP"] != null && 
                        int.TryParse(selectedRow["Mã SP"].ToString(), out int productId))
                    {
                        SanPhamForm editForm = new SanPhamForm(productId); // Edit mode
                        if (editForm.ShowDialog() == DialogResult.OK)
                        {
                            // Reload lại danh sách sản phẩm
                            await LoadProductsAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Lỗi khi mở form chỉnh sửa sản phẩm: {ex.Message}",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Xóa - Xóa sản phẩm đã chọn
        /// </summary>
        private async void ButtonXoa_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có sản phẩm nào được chọn không
            if (dataGridViewSanPham.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn ít nhất một sản phẩm để xóa.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            // Xác nhận xóa
            int selectedCount = dataGridViewSanPham.SelectedRows.Count;
            DialogResult confirmResult = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {selectedCount} sản phẩm đã chọn?",
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
                // Lấy danh sách Product ID từ bindingSource
                List<int> productIds = new List<int>();

                foreach (DataGridViewRow row in dataGridViewSanPham.SelectedRows)
                {
                    // Lấy DataRowView từ bindingSource
                    DataRowView rowView = (DataRowView)row.DataBoundItem;
                    
                    if (rowView["Mã SP"] != null &&
                        int.TryParse(rowView["Mã SP"].ToString(), out int productId))
                    {
                        productIds.Add(productId);
                    }
                }

                if (productIds.Count == 0)
                {
                    MessageBox.Show(
                        "Không thể lấy thông tin sản phẩm từ các dòng được chọn.",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                // Thực hiện xóa
                var deleteResult = await sanPhamModels.DeleteProductsAsync(productIds);

                // Hiển thị kết quả
                string message = $"Đã xóa thành công {deleteResult.SuccessCount} sản phẩm.";

                if (deleteResult.FailedProducts.Count > 0)
                {
                    message += $"\n\nKhông thể xóa {deleteResult.FailedProducts.Count} sản phẩm:";

                    foreach (var failed in deleteResult.FailedProducts)
                    {
                        message += $"\n- {failed.ProductName} (Mã SP: {failed.ProductId}): {failed.Reason}";

                        // Hiển thị thông tin OrderDetails nếu có
                        if (failed.RelatedOrderDetails.Count > 0)
                        {
                            message += "\n  Chi tiết hóa đơn liên quan:";
                            foreach (var orderDetail in failed.RelatedOrderDetails.Take(5)) // Chỉ hiển thị 5 cái đầu
                            {
                                message += $"\n    • HĐ: {orderDetail.OrderCode}, SKU: {orderDetail.VariantSKU}, SL: {orderDetail.Quantity}";
                            }
                            if (failed.RelatedOrderDetails.Count > 5)
                            {
                                message += $"\n    ... và {failed.RelatedOrderDetails.Count - 5} chi tiết khác";
                            }
                        }
                    }
                }

                MessageBox.Show(
                    message,
                    deleteResult.FailedProducts.Count > 0 ? "Kết quả xóa" : "Thành công",
                    MessageBoxButtons.OK,
                    deleteResult.FailedProducts.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information
                );

                // Reload danh sách sản phẩm
                if (deleteResult.SuccessCount > 0)
                {
                    await LoadProductsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi xóa sản phẩm: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void buttonDanhMuc_Click(object sender, EventArgs e)
        {
            try
            {
                SanPhamDanhMuc danhMucForm = new SanPhamDanhMuc();
                danhMucForm.ShowDialog(); // Hiển thị form dạng modal
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi mở form danh mục: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
