using System;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace helloworld
{
    /// <summary>
    /// Data Access Layer cho Categories
    /// Chỉ chứa các thao tác database, không có business logic
    /// </summary>
    internal class CategoryDAL
    {
        private ConnectDatabase database;

        public CategoryDAL()
        {
            database = new ConnectDatabase();
        }

        /// <summary>
        /// Lấy danh sách danh mục từ database
        /// </summary>
        public async Task<DataTable> LoadCategoriesAsync()
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    DataTable dataTable = new DataTable();

                    string query = @"
                        SELECT 
                            category_id AS 'Mã DM',
                            category_name AS 'Tên danh mục',
                            description AS 'Mô tả',
                            created_at AS 'Ngày tạo',
                            updated_at AS 'Ngày cập nhật'
                        FROM Categories
                        ORDER BY category_name";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                    return dataTable;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách danh mục: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách danh mục dạng List<Category> (dùng cho ComboBox)
        /// </summary>
        public async Task<List<Category>> GetCategoriesListAsync()
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    List<Category> list = new List<Category>();
                    string query = "SELECT category_id, category_name FROM Categories ORDER BY category_name";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                list.Add(new Category
                                {
                                    CategoryId = reader.GetInt32("category_id"),
                                    CategoryName = reader.GetString("category_name")
                                });
                            }
                        }
                    }
                    return list;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách danh mục: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin danh mục
        /// </summary>
        public async Task UpdateCategoryAsync(int categoryId, string categoryName, string? description)
        {
            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        UPDATE Categories 
                        SET category_name = @category_name, 
                            description = @description,
                            updated_at = CURRENT_TIMESTAMP
                        WHERE category_id = @category_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@category_id", categoryId);
                        command.Parameters.AddWithValue("@category_name", categoryName.Trim());
                        command.Parameters.AddWithValue("@description", string.IsNullOrWhiteSpace(description) ? DBNull.Value : description.Trim());

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            throw new Exception("Không tìm thấy danh mục để cập nhật.");
                        }
                    }
                });
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                {
                    throw new Exception($"Tên danh mục đã tồn tại trong hệ thống.", ex);
                }
                throw new Exception($"Lỗi khi cập nhật danh mục: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật danh mục: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm danh mục mới vào database
        /// </summary>
        public async Task<int> AddCategoryAsync(string categoryName, string? description)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        INSERT INTO Categories (category_name, description)
                        VALUES (@category_name, @description)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@category_name", categoryName.Trim());
                        command.Parameters.AddWithValue("@description", string.IsNullOrWhiteSpace(description) ? DBNull.Value : description.Trim());

                        await command.ExecuteNonQueryAsync();

                        command.CommandText = "SELECT LAST_INSERT_ID()";
                        object? result = await command.ExecuteScalarAsync();

                        if (result != null && int.TryParse(result.ToString(), out int categoryId))
                        {
                            return categoryId;
                        }
                        throw new Exception("Không thể lấy Category ID sau khi thêm danh mục.");
                    }
                });
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                {
                    throw new Exception($"Tên danh mục đã tồn tại trong hệ thống.", ex);
                }
                throw new Exception($"Lỗi khi thêm danh mục: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm danh mục: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa danh mục khỏi database
        /// </summary>
        public async Task DeleteCategoryAsync(int categoryId)
        {
            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = "DELETE FROM Categories WHERE category_id = @category_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@category_id", categoryId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            throw new Exception("Không tìm thấy danh mục để xóa.");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa danh mục: {ex.Message}", ex);
            }
        }
    }
}

