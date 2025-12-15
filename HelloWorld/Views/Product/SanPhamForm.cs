using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace helloworld
{
    public partial class SanPhamForm : Form
    {
        private ProductBLL sanPhamModels; // Đã chuyển sang ProductBLL
        private string? selectedImagePath = null;
        private int? editingProductId = null; // ID sản phẩm đang chỉnh sửa (null nếu là Add mode)
        private string? originalImagePath = null; // Ảnh gốc từ database
        private BindingSource variantsBindingSource = new BindingSource(); // BindingSource cho ProductVariants
        private DataTable variantsDataTable; // Lưu DataTable để có thể reload

        /// <summary>
        /// Constructor cho Add mode (thêm mới sản phẩm)
        /// </summary>
        public SanPhamForm() : this(null)
        {
        }

        /// <summary>
        /// Constructor với productId - nếu có thì là Edit mode, không có thì là Add mode
        /// </summary>
        /// <param name="productId">ID sản phẩm cần chỉnh sửa (null nếu thêm mới)</param>
        public SanPhamForm(int? productId)
        {
            InitializeComponent();
            sanPhamModels = new ProductBLL();
            editingProductId = productId;

            // Khởi tạo BindingSource và gắn vào DataGridView
            dataGridViewSanPhamChiTiet.DataSource = variantsBindingSource;
            dataGridViewSanPhamChiTiet.CellDoubleClick += DataGridViewSanPhamChiTiet_CellDoubleClick;

            this.Load += SanPhamForm_Load;
            buttonLuu.Click += ButtonLuu_Click;
            pictureBoxAnhSanPham.Click += PictureBoxAnhSanPham_Click;
            pictureBoxAnhSanPham.Cursor = Cursors.Hand;
            buttonThemSanPhamChiTiet.Click += ButtonThemSanPhamChiTiet_Click;
            // buttonXoaSanPhamChiTiet.Click đã được đăng ký trong Designer.cs, không cần đăng ký lại

            // Nếu là Edit mode, load dữ liệu sản phẩm
            if (editingProductId.HasValue)
            {
                this.Text = "Chỉnh sửa sản phẩm";
            }
            else
            {
                this.Text = "Thêm mới sản phẩm";
                comboBox1.SelectedIndex = 0; // Mặc định "Đang bán"
            }
        }

        /// <summary>
        /// Load form - Tạo mã sản phẩm và load danh mục hoặc load dữ liệu sản phẩm nếu edit
        /// </summary>
        private async void SanPhamForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Load danh sách danh mục trước
                await LoadCategoriesAsync();

                if (editingProductId.HasValue)
                {
                    // Edit mode: Load dữ liệu sản phẩm
                    await LoadProductDataAsync(editingProductId.Value);
                }
                else
                {
                    // Add mode: Tạo mã sản phẩm tự động
                    string productCode = await sanPhamModels.GenerateProductCodeAsync();
                    labelMaHoaDon.Text = productCode;

                    // Hiển thị ngày tạo
                    labelNgayTao.Text = $"Ngày tạo: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    label11.Text = $"Ngày cập nhật: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

                    // Set giá trị mặc định cho các textbox tổng hợp (Add mode chưa có variants)
                    textBoxTongTonKho.Text = "0";
                    textBoxSoLuongBan.Text = "0";
                    textBoxTienBanDuoc.Text = "0";

                    // Set readonly cho các textbox này
                    textBoxTongTonKho.ReadOnly = true;
                    textBoxSoLuongBan.ReadOnly = true;
                    textBoxTienBanDuoc.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi tạo form: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load dữ liệu sản phẩm từ database để hiển thị trong form (Edit mode)
        /// </summary>
        private async Task LoadProductDataAsync(int productId)
        {
            try
            {
                Product? product = await sanPhamModels.GetProductByIdAsync(productId);

                if (product == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Điền dữ liệu vào form
                labelMaHoaDon.Text = product.ProductCode;
                textBoxTenSanPham.Text = product.ProductName;

                // Chọn danh mục
                if (product.CategoryId.HasValue)
                {
                    comboBoxDanhMuc.SelectedValue = product.CategoryId.Value;
                }

                // Chọn trạng thái
                comboBox1.SelectedIndex = product.IsActive ? 0 : 1; // 0 = "Đang bán", 1 = "Ngừng bán"

                // Xử lý ảnh
                if (!string.IsNullOrWhiteSpace(product.BaseImagePath))
                {
                    originalImagePath = product.BaseImagePath;
                    selectedImagePath = product.BaseImagePath;

                    // Hiển thị ảnh nếu file tồn tại
                    if (File.Exists(product.BaseImagePath))
                    {
                        try
                        {
                            Image image = Image.FromFile(product.BaseImagePath);
                            pictureBoxAnhSanPham.Image = image;
                            pictureBoxAnhSanPham.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                        catch
                        {
                            // Nếu không load được ảnh, bỏ qua
                        }
                    }
                }

                // Hiển thị ngày tạo và cập nhật
                labelNgayTao.Text = $"Ngày tạo: {product.CreatedAt:dd/MM/yyyy HH:mm:ss}";
                label11.Text = $"Ngày cập nhật: {product.UpdatedAt:dd/MM/yyyy HH:mm:ss}";

                // Load danh sách biến thể sản phẩm
                await LoadProductVariantsAsync(productId);

                // Load và hiển thị thông tin tổng hợp (tổng tồn kho, số lượng bán, tiền bán)
                await LoadProductSummaryAsync(productId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu sản phẩm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load danh sách ProductVariants và hiển thị trong DataGridView
        /// Sử dụng BindingSource để quản lý dữ liệu
        /// </summary>
        private async Task LoadProductVariantsAsync(int productId)
        {
            try
            {
                // Load dữ liệu từ database
                variantsDataTable = await sanPhamModels.LoadProductVariantsAsync(productId);

                // Bind vào BindingSource
                variantsBindingSource.DataSource = variantsDataTable;

                // DataGridView đã được bind với variantsBindingSource trong constructor
                // Không cần set lại DataSource

                // Cấu hình DataGridView
                ConfigureProductVariantsDataGridView();

                // Cập nhật lại thông tin tổng hợp sau khi load variants
                if (editingProductId.HasValue)
                {
                    await LoadProductSummaryAsync(productId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi tải danh sách biến thể sản phẩm: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Load và hiển thị thông tin tổng hợp của sản phẩm (tổng tồn kho, số lượng bán, tiền bán)
        /// </summary>
        private async Task LoadProductSummaryAsync(int productId)
        {
            try
            {
                // Chỉ load khi đang ở Edit mode (có productId)
                if (!editingProductId.HasValue || editingProductId.Value != productId)
                {
                    return;
                }

                // Lấy thông tin tổng hợp từ database
                ProductSummary summary = await sanPhamModels.GetProductSummaryAsync(productId);

                // Hiển thị vào các textbox
                textBoxTongTonKho.Text = summary.TotalStockQuantity.ToString("N0");
                textBoxSoLuongBan.Text = summary.TotalSalesQuantity.ToString("N0");
                textBoxTienBanDuoc.Text = summary.TotalSalesAmount.ToString("N0");

                // Set readonly cho các textbox này (chỉ hiển thị, không cho sửa)
                textBoxTongTonKho.ReadOnly = true;
                textBoxSoLuongBan.ReadOnly = true;
                textBoxTienBanDuoc.ReadOnly = true;
            }
            catch (Exception ex)
            {
                // Không hiển thị lỗi để tránh làm gián đoạn, chỉ log
                // Có thể thêm logging sau
            }
        }

        /// <summary>
        /// Cấu hình hiển thị cho DataGridView ProductVariants
        /// </summary>
        private void ConfigureProductVariantsDataGridView()
        {
            // Tự động điều chỉnh độ rộng cột
            dataGridViewSanPhamChiTiet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Cho phép chọn nhiều dòng
            dataGridViewSanPhamChiTiet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewSanPhamChiTiet.MultiSelect = true;

            // Ẩn cột row header mặc định
            dataGridViewSanPhamChiTiet.RowHeadersVisible = false;

            // Cho phép đọc-only (không cho chỉnh sửa trực tiếp)
            dataGridViewSanPhamChiTiet.ReadOnly = true;

            // Căn giữa header
            dataGridViewSanPhamChiTiet.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Style cho header
            dataGridViewSanPhamChiTiet.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            // Format cột số
            if (dataGridViewSanPhamChiTiet.Columns["Số lượng tồn kho"] != null)
            {
                dataGridViewSanPhamChiTiet.Columns["Số lượng tồn kho"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridViewSanPhamChiTiet.Columns["Số lượng tồn kho"].DefaultCellStyle.Format = "N0";
            }

            if (dataGridViewSanPhamChiTiet.Columns["Số lượng đã bán"] != null)
            {
                dataGridViewSanPhamChiTiet.Columns["Số lượng đã bán"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridViewSanPhamChiTiet.Columns["Số lượng đã bán"].DefaultCellStyle.Format = "N0";
            }

            // Format cột tiền tệ
            if (dataGridViewSanPhamChiTiet.Columns["Giá nhập"] != null)
            {
                dataGridViewSanPhamChiTiet.Columns["Giá nhập"].DefaultCellStyle.Format = "N0";
                dataGridViewSanPhamChiTiet.Columns["Giá nhập"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dataGridViewSanPhamChiTiet.Columns["Giá bán"] != null)
            {
                dataGridViewSanPhamChiTiet.Columns["Giá bán"].DefaultCellStyle.Format = "N0";
                dataGridViewSanPhamChiTiet.Columns["Giá bán"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dataGridViewSanPhamChiTiet.Columns["Số tiền đã bán"] != null)
            {
                dataGridViewSanPhamChiTiet.Columns["Số tiền đã bán"].DefaultCellStyle.Format = "N0";
                dataGridViewSanPhamChiTiet.Columns["Số tiền đã bán"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            // Căn giữa cho các cột text
            if (dataGridViewSanPhamChiTiet.Columns["SKU"] != null)
            {
                dataGridViewSanPhamChiTiet.Columns["SKU"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dataGridViewSanPhamChiTiet.Columns["Size"] != null)
            {
                dataGridViewSanPhamChiTiet.Columns["Size"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dataGridViewSanPhamChiTiet.Columns["Màu"] != null)
            {
                dataGridViewSanPhamChiTiet.Columns["Màu"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dataGridViewSanPhamChiTiet.Columns["Trạng thái"] != null)
            {
                dataGridViewSanPhamChiTiet.Columns["Trạng thái"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // Ẩn cột Mã biến thể (chỉ dùng để xử lý logic, không cần hiển thị)
            if (dataGridViewSanPhamChiTiet.Columns["Mã biến thể"] != null)
            {
                dataGridViewSanPhamChiTiet.Columns["Mã biến thể"].Visible = false;
            }

            // Ẩn cột ngày tạo và ngày cập nhật nếu không cần thiết (có thể bỏ comment nếu muốn hiển thị)
            if (dataGridViewSanPhamChiTiet.Columns["Ngày tạo"] != null)
            {
                dataGridViewSanPhamChiTiet.Columns["Ngày tạo"].Visible = false;
            }

            if (dataGridViewSanPhamChiTiet.Columns["Ngày cập nhật"] != null)
            {
                dataGridViewSanPhamChiTiet.Columns["Ngày cập nhật"].Visible = false;
            }
        }

        /// <summary>
        /// Load danh sách danh mục vào ComboBox
        /// </summary>
        private async Task LoadCategoriesAsync()
        {
            try
            {
                List<Category> categories = await sanPhamModels.LoadCategoriesAsync();

                comboBoxDanhMuc.DataSource = categories;
                comboBoxDanhMuc.DisplayMember = "CategoryName";
                comboBoxDanhMuc.ValueMember = "CategoryId";
                comboBoxDanhMuc.SelectedIndex = -1; // Không chọn mặc định
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh mục: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xử lý khi click vào pictureBox để chọn ảnh
        /// </summary>
        private void PictureBoxAnhSanPham_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        selectedImagePath = openFileDialog.FileName;

                        // Hiển thị ảnh preview
                        if (File.Exists(selectedImagePath))
                        {
                            Image image = Image.FromFile(selectedImagePath);
                            pictureBoxAnhSanPham.Image = image;
                            pictureBoxAnhSanPham.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi tải ảnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Lưu sản phẩm (Add hoặc Update)
        /// </summary>
        private async void ButtonLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(textBoxTenSanPham.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên sản phẩm.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBoxTenSanPham.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(labelMaHoaDon.Text))
                {
                    MessageBox.Show("Mã sản phẩm không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Lấy thông tin từ form
                string productName = textBoxTenSanPham.Text.Trim();
                int? categoryId = null;

                if (comboBoxDanhMuc.SelectedValue != null && comboBoxDanhMuc.SelectedValue != DBNull.Value)
                {
                    categoryId = Convert.ToInt32(comboBoxDanhMuc.SelectedValue);
                }

                string? description = null; // Có thể thêm TextBox cho mô tả sau

                // Xử lý ảnh: Nếu không chọn ảnh mới và đang ở Edit mode, giữ nguyên ảnh cũ
                string? baseImagePath = selectedImagePath;
                if (editingProductId.HasValue && selectedImagePath == null && !string.IsNullOrWhiteSpace(originalImagePath))
                {
                    // Đang edit và không chọn ảnh mới, giữ nguyên ảnh cũ
                    baseImagePath = originalImagePath;
                }

                bool isActive = comboBox1.SelectedItem?.ToString() == "Đang bán";

                if (editingProductId.HasValue)
                {
                    // Update mode
                    await sanPhamModels.UpdateProductAsync(
                        editingProductId.Value,
                        productName,
                        categoryId,
                        description,
                        baseImagePath,
                        isActive
                    );

                    MessageBox.Show(
                        $"Cập nhật sản phẩm thành công!",
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    // Add mode
                    string productCode = labelMaHoaDon.Text;
                    int productId = await sanPhamModels.AddProductAsync(
                        productCode,
                        productName,
                        categoryId,
                        description,
                        baseImagePath,
                        isActive
                    );

                    MessageBox.Show(
                        $"Thêm sản phẩm thành công!\nMã sản phẩm: {productCode}\nProduct ID: {productId}",
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu sản phẩm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Thêm chi tiết - Mở form SanPhamChiTietForm
        /// </summary>
        private async void ButtonThemSanPhamChiTiet_Click(object sender, EventArgs e)
        {
            try
            {
                // Chỉ cho phép thêm variant khi đang ở Edit mode (có productId)
                if (!editingProductId.HasValue)
                {
                    MessageBox.Show(
                        "Vui lòng lưu sản phẩm trước khi thêm biến thể.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                // Mở form thêm biến thể sản phẩm
                SanPhamChiTietForm addVariantForm = new SanPhamChiTietForm(editingProductId.Value);
                DialogResult result = addVariantForm.ShowDialog();

                // Nếu thêm thành công, reload danh sách biến thể
                if (result == DialogResult.OK)
                {
                    await LoadProductVariantsAsync(editingProductId.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi mở form thêm biến thể sản phẩm: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Xóa - Xóa các biến thể đã chọn (đơn giản như SanPhamViews)
        /// </summary>
        private async void buttonXoaSanPhamChiTiet_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có biến thể nào được chọn không
            if (dataGridViewSanPhamChiTiet.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn ít nhất một biến thể để xóa.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            // Xác nhận xóa
            int selectedCount = dataGridViewSanPhamChiTiet.SelectedRows.Count;
            DialogResult confirmResult = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {selectedCount} biến thể đã chọn?",
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
                // Lấy danh sách Variant ID từ bindingSource
                List<int> variantIds = new List<int>();

                foreach (DataGridViewRow row in dataGridViewSanPhamChiTiet.SelectedRows)
                {
                    // Sử dụng DataBoundItem từ bindingSource
                    DataRowView rowView = (DataRowView)row.DataBoundItem;
                    
                    if (rowView["Mã biến thể"] != null &&
                        int.TryParse(rowView["Mã biến thể"].ToString(), out int variantId))
                    {
                        variantIds.Add(variantId);
                    }
                }

                if (variantIds.Count == 0)
                {
                    MessageBox.Show(
                        "Không thể lấy thông tin biến thể từ các dòng được chọn.",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                // Thực hiện xóa (model tự xử lý tất cả logic kiểm tra và xóa)
                var deleteResult = await sanPhamModels.DeleteProductVariantsAsync(variantIds);

                // Hiển thị kết quả
                string message = $"Đã xóa thành công {deleteResult.SuccessCount} biến thể.";

                if (deleteResult.FailedVariants.Count > 0)
                {
                    message += $"\n\nKhông thể xóa {deleteResult.FailedVariants.Count} biến thể:";
                    foreach (var failed in deleteResult.FailedVariants)
                    {
                        message += $"\n- {failed.SKU} (Mã: {failed.VariantId}): {failed.Reason}";
                    }
                }

                MessageBox.Show(
                    message,
                    deleteResult.FailedVariants.Count > 0 ? "Kết quả xóa" : "Thành công",
                    MessageBoxButtons.OK,
                    deleteResult.FailedVariants.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information
                );

                // Reload danh sách biến thể
                if (deleteResult.SuccessCount > 0 && editingProductId.HasValue)
                {
                    await LoadProductVariantsAsync(editingProductId.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi xóa biến thể sản phẩm: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void dataGridViewSanPhamChiTiet_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        /// <summary>
        /// Mở form chỉnh sửa biến thể khi double-click vào dòng
        /// </summary>
        private async void DataGridViewSanPhamChiTiet_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (!editingProductId.HasValue)
            {
                MessageBox.Show("Vui lòng lưu sản phẩm trước khi chỉnh sửa biến thể.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                DataGridViewRow row = dataGridViewSanPhamChiTiet.Rows[e.RowIndex];
                int variantId = 0;

                // Ưu tiên lấy từ DataRowView để tránh lỗi tên cột
                if (row.DataBoundItem is DataRowView rowView &&
                    rowView["Mã biến thể"] != null &&
                    int.TryParse(rowView["Mã biến thể"].ToString(), out int parsedId))
                {
                    variantId = parsedId;
                }
                else if (row.Cells["Mã biến thể"] != null &&
                         int.TryParse(row.Cells["Mã biến thể"].Value?.ToString(), out int cellId))
                {
                    variantId = cellId;
                }

                if (variantId <= 0)
                {
                    MessageBox.Show("Không thể xác định mã biến thể để chỉnh sửa.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var editForm = new SanPhamChiTietForm(editingProductId.Value, variantId);
                DialogResult result = editForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    await LoadProductVariantsAsync(editingProductId.Value);
                    await LoadProductSummaryAsync(editingProductId.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form chỉnh sửa biến thể: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
