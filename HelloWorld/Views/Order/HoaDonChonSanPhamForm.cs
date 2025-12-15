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
    public partial class HoaDonChonSanPhamForm : Form
    {
        private OrderBLL hoaDonModels; // Đã chuyển sang OrderBLL
        private DataTable productsTable;
        private DataView productsView;
        private BindingSource bindingSource = new BindingSource(); // BindingSource cho Products
        private List<ProductVariantInfo> selectedVariants;

        /// <summary>
        /// Danh sách sản phẩm đã chọn
        /// </summary>
        public List<ProductVariantInfo> SelectedVariants => selectedVariants;

        public HoaDonChonSanPhamForm()
        {
            InitializeComponent();
            hoaDonModels = new OrderBLL(); // Đã chuyển sang OrderBLL
            selectedVariants = new List<ProductVariantInfo>();
            
            // Khởi tạo BindingSource và gắn vào DataGridView
            // QUAN TRỌNG: Phải set DataSource trước khi load dữ liệu
            dataGridView1.DataSource = bindingSource;
            
            this.Load += HoaDonChonSanPhamForm_Load;
            buttonThemSanPham.Click += ButtonThemSanPham_Click;
            buttonHuy.Click += ButtonHuy_Click;
            textBoxTimKiem.TextChanged += TextBoxTimKiem_TextChanged;
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
        }

        /// <summary>
        /// Load danh sách sản phẩm khi form được mở
        /// </summary>
        private async void HoaDonChonSanPhamForm_Load(object sender, EventArgs e)
        {
            await LoadProductsAsync();
        }

        /// <summary>
        /// Tải danh sách sản phẩm từ database
        /// </summary>
        private async Task LoadProductsAsync()
        {
            try
            {
                // Hiển thị loading
                this.Cursor = Cursors.WaitCursor;
                dataGridView1.DataSource = null;
                dataGridView1.Refresh();

                // Lấy danh sách sản phẩm
                List<ProductVariantInfo> variants = await hoaDonModels.GetProductVariantsAsync();

                if (variants == null || variants.Count == 0)
                {
                    MessageBox.Show("Không có sản phẩm nào trong kho.\n\nVui lòng kiểm tra:\n- Sản phẩm có được kích hoạt (is_active = TRUE) không?\n- Có biến thể sản phẩm nào trong database không?", 
                        "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Cursor = Cursors.Default;
                    return;
                }

                // Tạo DataTable
                productsTable = new DataTable();
                productsTable.Columns.Add("VariantId", typeof(int));
                productsTable.Columns.Add("SKU", typeof(string));
                productsTable.Columns.Add("Tên sản phẩm", typeof(string));
                productsTable.Columns.Add("Size", typeof(string));
                productsTable.Columns.Add("Màu", typeof(string));
                productsTable.Columns.Add("Giá bán", typeof(decimal));
                productsTable.Columns.Add("Tồn kho", typeof(int));

                foreach (var variant in variants)
                {
                    productsTable.Rows.Add(
                        variant.VariantId,
                        variant.SKU ?? "",
                        variant.ProductName ?? "",
                        variant.Size ?? "",
                        variant.Color ?? "",
                        variant.SellingPrice,
                        variant.StockQuantity
                    );
                }

                // Tạo DataView để hỗ trợ tìm kiếm mượt mà
                productsView = productsTable.DefaultView;
                
                // QUAN TRỌNG: Suspend layout để tăng hiệu suất khi bind dữ liệu
                dataGridView1.SuspendLayout();
                
                try
                {
                    // Bind DataView vào BindingSource
                    bindingSource.DataSource = productsView;
                    
                    // Đảm bảo DataGridView được bind đúng
                    // Nếu DataGridView chưa được bind, bind lại
                    if (dataGridView1.DataSource != bindingSource)
                    {
                        dataGridView1.DataSource = bindingSource;
                    }
                    
                    // Cấu hình DataGridView (phải gọi sau khi bind dữ liệu)
                    ConfigureDataGridView();
                }
                finally
                {
                    // Resume layout sau khi bind xong
                    dataGridView1.ResumeLayout(false);
                    dataGridView1.PerformLayout();
                }
                
                // Refresh DataGridView để hiển thị dữ liệu
                dataGridView1.Refresh();
                
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                string errorMessage = $"Lỗi khi tải danh sách sản phẩm:\n\n{ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nChi tiết: {ex.InnerException.Message}";
                }
                MessageBox.Show(errorMessage, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Cấu hình DataGridView
        /// </summary>
        private void ConfigureDataGridView()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = true; // Cho phép chọn nhiều dòng
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;

            // Ẩn cột VariantId và SKU (chỉ dùng để xử lý)
            if (dataGridView1.Columns["VariantId"] != null)
            {
                dataGridView1.Columns["VariantId"].Visible = false;
            }

            if (dataGridView1.Columns["SKU"] != null)
            {
                dataGridView1.Columns["SKU"].Visible = false;
            }

            // Format cột giá bán
            if (dataGridView1.Columns["Giá bán"] != null)
            {
                dataGridView1.Columns["Giá bán"].DefaultCellStyle.Format = "N0";
                dataGridView1.Columns["Giá bán"].DefaultCellStyle.Alignment = 
                    DataGridViewContentAlignment.MiddleRight;
            }

            // Format cột tồn kho
            if (dataGridView1.Columns["Tồn kho"] != null)
            {
                dataGridView1.Columns["Tồn kho"].DefaultCellStyle.Alignment = 
                    DataGridViewContentAlignment.MiddleCenter;
            }

            // Format cột Size và Màu
            if (dataGridView1.Columns["Size"] != null)
            {
                dataGridView1.Columns["Size"].DefaultCellStyle.Alignment = 
                    DataGridViewContentAlignment.MiddleCenter;
            }

            if (dataGridView1.Columns["Màu"] != null)
            {
                dataGridView1.Columns["Màu"].DefaultCellStyle.Alignment = 
                    DataGridViewContentAlignment.MiddleCenter;
            }
        }

        /// <summary>
        /// Tìm kiếm sản phẩm theo từ khóa - Tìm kiếm mượt mà với DataView
        /// </summary>
        private void TextBoxTimKiem_TextChanged(object sender, EventArgs e)
        {
            if (productsTable == null || productsView == null)
                return;

            string keyword = textBoxTimKiem.Text.Trim();
            
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    // Xóa filter để hiển thị tất cả dữ liệu
                    productsView.RowFilter = string.Empty;
                }
                else
                {
                    // Escape các ký tự đặc biệt trong DataView RowFilter
                    keyword = keyword.Replace("[", "[[]")
                                    .Replace("]", "[]]")
                                    .Replace("*", "[*]")
                                    .Replace("%", "[%]")
                                    .Replace("'", "''")
                                    .Replace("#", "[#]");

                    // Lọc dữ liệu theo SKU, Tên sản phẩm, Size, Màu
                    string filter = $"(SKU LIKE '%{keyword}%' OR [Tên sản phẩm] LIKE '%{keyword}%' OR Size LIKE '%{keyword}%' OR Màu LIKE '%{keyword}%')";
                    productsView.RowFilter = filter;
                }
            }
            catch (Exception ex)
            {
                // Nếu có lỗi trong filter, xóa filter và hiển thị tất cả
                productsView.RowFilter = string.Empty;
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Double-click để thêm sản phẩm
        /// </summary>
        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                ButtonThemSanPham_Click(sender, e);
            }
        }

        /// <summary>
        /// Thêm sản phẩm đã chọn vào danh sách
        /// </summary>
        private void ButtonThemSanPham_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một sản phẩm.", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            selectedVariants.Clear();
            List<string> errorMessages = new List<string>();

            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                // Sử dụng DataBoundItem từ bindingSource
                DataRowView rowView = (DataRowView)row.DataBoundItem;
                
                int variantId = Convert.ToInt32(rowView["VariantId"]);
                string sku = rowView["SKU"].ToString();
                string productName = rowView["Tên sản phẩm"].ToString();
                string size = rowView["Size"]?.ToString() ?? "";
                string color = rowView["Màu"]?.ToString() ?? "";
                decimal price = Convert.ToDecimal(rowView["Giá bán"]);
                int stock = Convert.ToInt32(rowView["Tồn kho"]);

                // Kiểm tra tồn kho
                if (stock <= 0)
                {
                    errorMessages.Add($"Sản phẩm '{productName}' ({size}/{color}) đã hết hàng.");
                    continue;
                }

                // Thêm vào danh sách đã chọn
                selectedVariants.Add(new ProductVariantInfo
                {
                    VariantId = variantId,
                    SKU = sku,
                    ProductName = productName,
                    Size = size,
                    Color = color,
                    SellingPrice = price,
                    StockQuantity = stock
                });
            }

            // Hiển thị cảnh báo nếu có sản phẩm hết hàng
            if (errorMessages.Count > 0)
            {
                string message = "Các sản phẩm sau không thể thêm:\n" + string.Join("\n", errorMessages);
                MessageBox.Show(message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Nếu có ít nhất một sản phẩm hợp lệ, đóng form
            if (selectedVariants.Count > 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        /// <summary>
        /// Hủy và đóng form
        /// </summary>
        private void ButtonHuy_Click(object sender, EventArgs e)
        {
            selectedVariants.Clear();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
