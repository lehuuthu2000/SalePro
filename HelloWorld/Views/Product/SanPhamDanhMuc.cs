using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using helloworld.DAL;

namespace helloworld
{
    public partial class SanPhamDanhMuc : Form
    {
        private CategoryBLL danhMucModels; // Đã chuyển sang CategoryBLL
        private bool isUpdating = false; // Flag để tránh vòng lặp khi cập nhật

        public SanPhamDanhMuc()
        {
            InitializeComponent();
            danhMucModels = new CategoryBLL(); // Đã chuyển sang CategoryBLL
            this.Load += SanPhamDanhMuc_Load;
            textBoxTimKiem.TextChanged += TextBoxTimKiem_TextChanged;
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
        }

        /// <summary>
        /// Load danh sách danh mục khi form được mở
        /// </summary>
        private async void SanPhamDanhMuc_Load(object sender, EventArgs e)
        {
            await LoadCategoriesAsync();
        }

        /// <summary>
        /// Load danh sách danh mục từ database và hiển thị vào DataGridView
        /// </summary>
        private async Task LoadCategoriesAsync()
        {
            try
            {
                // Hiển thị loading indicator
                dataGridView1.DataSource = null;
                dataGridView1.Refresh();

                // Gọi method từ DanhMucModels để lấy dữ liệu
                DataTable dataTable = await danhMucModels.LoadCategoriesAsync();

                // Bind dữ liệu vào DataGridView
                dataGridView1.DataSource = dataTable;

                // Cấu hình DataGridView
                ConfigureDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi tải danh sách danh mục: {ex.Message}",
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

            // Cho phép chỉnh sửa trực tiếp (chỉ cho cột Tên danh mục và Mô tả)
            dataGridView1.ReadOnly = false;

            // Căn giữa header
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Style cho header
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            // Cấu hình từng cột
            if (dataGridView1.Columns["Mã DM"] != null)
            {
                dataGridView1.Columns["Mã DM"].ReadOnly = true; // Không cho sửa mã
                dataGridView1.Columns["Mã DM"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dataGridView1.Columns["Tên danh mục"] != null)
            {
                dataGridView1.Columns["Tên danh mục"].ReadOnly = false; // Cho phép sửa
            }

            if (dataGridView1.Columns["Mô tả"] != null)
            {
                dataGridView1.Columns["Mô tả"].ReadOnly = false; // Cho phép sửa
            }

            if (dataGridView1.Columns["Ngày tạo"] != null)
            {
                dataGridView1.Columns["Ngày tạo"].ReadOnly = true; // Không cho sửa
            }

            if (dataGridView1.Columns["Ngày cập nhật"] != null)
            {
                dataGridView1.Columns["Ngày cập nhật"].ReadOnly = true; // Không cho sửa
            }
        }

        /// <summary>
        /// Tìm kiếm danh mục khi người dùng nhập từ khóa
        /// </summary>
        private async void TextBoxTimKiem_TextChanged(object sender, EventArgs e)
        {
            // Tạm thời không implement tìm kiếm, có thể thêm sau nếu cần
            // await SearchAndBindAsync(textBoxTimKiem.Text);
        }

        /// <summary>
        /// Xử lý sự kiện khi giá trị cell thay đổi - Tự động lưu
        /// </summary>
        private async void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Chỉ xử lý khi người dùng sửa cột "Tên danh mục" hoặc "Mô tả"
            if (isUpdating || e.RowIndex < 0)
                return;

            string columnName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (columnName != "Tên danh mục" && columnName != "Mô tả")
                return;

            try
            {
                // Lấy thông tin từ dòng được sửa
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                
                if (row.Cells["Mã DM"].Value == null)
                    return;

                if (!int.TryParse(row.Cells["Mã DM"].Value.ToString(), out int categoryId))
                    return;

                string categoryName = row.Cells["Tên danh mục"].Value?.ToString() ?? string.Empty;
                string description = row.Cells["Mô tả"].Value?.ToString() ?? string.Empty;

                // Validate
                if (string.IsNullOrWhiteSpace(categoryName))
                {
                    MessageBox.Show(
                        "Tên danh mục không được để trống.",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    // Reload để khôi phục giá trị cũ
                    await LoadCategoriesAsync();
                    return;
                }

                // Nếu categoryId = 0, đây là dòng mới - cần thêm mới
                if (categoryId == 0)
                {
                    // Thêm danh mục mới vào database
                    int newCategoryId = await danhMucModels.AddCategoryAsync(categoryName, description);
                    
                    // Cập nhật lại Mã DM trong DataGridView
                    isUpdating = true;
                    row.Cells["Mã DM"].Value = newCategoryId;
                    
                    // Cập nhật ngày tạo và ngày cập nhật
                    DatabaseContext db = new DatabaseContext();
                    try
                    {
                        var connection = await db.OpenConnectionAsync();
                        string query = "SELECT created_at, updated_at FROM Categories WHERE category_id = @category_id";
                        using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@category_id", newCategoryId);
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    row.Cells["Ngày tạo"].Value = reader.GetDateTime("created_at");
                                    row.Cells["Ngày cập nhật"].Value = reader.GetDateTime("updated_at");
                                }
                            }
                        }
                    }
                    finally
                    {
                        db.CloseConnection();
                        isUpdating = false;
                    }
                }
                else
                {
                    // Cập nhật danh mục hiện có vào database
                    await danhMucModels.UpdateCategoryAsync(categoryId, categoryName, description);

                    // Cập nhật lại cột "Ngày cập nhật" trong DataGridView
                    isUpdating = true;
                    DatabaseContext db = new DatabaseContext();
                    try
                    {
                        var connection = await db.OpenConnectionAsync();
                        string query = "SELECT updated_at FROM Categories WHERE category_id = @category_id";
                        using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@category_id", categoryId);
                            var result = await cmd.ExecuteScalarAsync();
                            if (result != null)
                            {
                                row.Cells["Ngày cập nhật"].Value = result;
                            }
                        }
                    }
                    finally
                    {
                        db.CloseConnection();
                        isUpdating = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi lưu danh mục: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                // Reload để khôi phục giá trị cũ
                await LoadCategoriesAsync();
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi kết thúc chỉnh sửa cell
        /// </summary>
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Có thể thêm logic validation tại đây nếu cần
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Thêm mới - Thêm dòng mới vào DataGridView
        /// </summary>
        private void buttonThemMoi_Click(object sender, EventArgs e)
        {
            try
            {
                // Thêm một dòng mới vào DataGridView
                DataTable dataTable = (DataTable)dataGridView1.DataSource;
                if (dataTable != null)
                {
                    DataRow newRow = dataTable.NewRow();
                    newRow["Mã DM"] = 0; // Tạm thời set 0, sẽ được cập nhật sau khi lưu
                    newRow["Tên danh mục"] = "";
                    newRow["Mô tả"] = "";
                    newRow["Ngày tạo"] = DateTime.Now;
                    newRow["Ngày cập nhật"] = DateTime.Now;
                    
                    dataTable.Rows.Add(newRow);
                    
                    // Chọn dòng vừa thêm và focus vào cột "Tên danh mục"
                    int newRowIndex = dataGridView1.Rows.Count - 1;
                    dataGridView1.CurrentCell = dataGridView1.Rows[newRowIndex].Cells["Tên danh mục"];
                    dataGridView1.BeginEdit(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi thêm dòng mới: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Lưu - Lưu tất cả thay đổi
        /// </summary>
        private async void buttonLuu_Click(object sender, EventArgs e)
        {
            // Vì đã tự động lưu khi sửa, nút này có thể dùng để reload hoặc thông báo
            await LoadCategoriesAsync();
            MessageBox.Show(
                "Đã làm mới danh sách danh mục.",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Xóa - Xóa danh mục đã chọn
        /// </summary>
        private async void buttonXoa_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có danh mục nào được chọn không
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn ít nhất một danh mục để xóa.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            // Xác nhận xóa
            int selectedCount = dataGridView1.SelectedRows.Count;
            DialogResult confirmResult = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {selectedCount} danh mục đã chọn?",
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
                // Lấy danh sách Category ID từ các dòng được chọn
                List<int> categoryIds = new List<int>();

                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    if (row.Cells["Mã DM"].Value != null &&
                        int.TryParse(row.Cells["Mã DM"].Value.ToString(), out int categoryId))
                    {
                        categoryIds.Add(categoryId);
                    }
                }

                if (categoryIds.Count == 0)
                {
                    MessageBox.Show(
                        "Không thể lấy thông tin danh mục từ các dòng được chọn.",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                // Thực hiện xóa từng danh mục
                int successCount = 0;
                int failCount = 0;
                string errorMessages = "";

                foreach (int categoryId in categoryIds)
                {
                    try
                    {
                        await danhMucModels.DeleteCategoryAsync(categoryId);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        errorMessages += $"\n- Mã DM {categoryId}: {ex.Message}";
                    }
                }

                // Hiển thị kết quả
                string message = $"Đã xóa thành công {successCount} danh mục.";
                if (failCount > 0)
                {
                    message += $"\n\nKhông thể xóa {failCount} danh mục:{errorMessages}";
                }

                MessageBox.Show(
                    message,
                    failCount > 0 ? "Kết quả xóa" : "Thành công",
                    MessageBoxButtons.OK,
                    failCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information
                );

                // Reload danh sách danh mục
                await LoadCategoriesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi xóa danh mục: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
