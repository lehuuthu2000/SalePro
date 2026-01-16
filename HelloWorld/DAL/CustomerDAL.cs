using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using helloworld.DAL;

namespace helloworld
{
    /// <summary>
    /// Data Access Layer cho Customers
    /// Chỉ chứa các thao tác database, không có business logic
    /// </summary>
    internal class CustomerDAL
    {
        private DatabaseContext database;

        public CustomerDAL()
        {
            database = new DatabaseContext();
        }

        /// <summary>
        /// Thêm khách hàng mới vào database
        /// </summary>
        public async Task<int> AddCustomerAsync(Customer customer)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        INSERT INTO Customers (full_name, phone_number, email, address, image, points)
                        VALUES (@full_name, @phone_number, @email, @address, @image, @points)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@full_name", customer.FullName.Trim());
                        command.Parameters.AddWithValue("@phone_number", customer.PhoneNumber.Trim());
                        command.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(customer.Email) ? DBNull.Value : customer.Email.Trim());
                        command.Parameters.AddWithValue("@address", string.IsNullOrWhiteSpace(customer.Address) ? DBNull.Value : customer.Address.Trim());
                        command.Parameters.AddWithValue("@image", customer.Image != null && customer.Image.Length > 0 ? (object)customer.Image : DBNull.Value);
                        command.Parameters.AddWithValue("@points", customer.Points);

                        await command.ExecuteNonQueryAsync();

                        command.CommandText = "SELECT LAST_INSERT_ID()";
                        object? result = await command.ExecuteScalarAsync();
                        
                        if (result != null && int.TryParse(result.ToString(), out int customerId))
                        {
                            return customerId;
                        }
                        throw new Exception("Không thể lấy được ID khách hàng vừa tạo.");
                    }
                });
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                {
                    throw new Exception("Số điện thoại này đã tồn tại trong hệ thống. Vui lòng sử dụng số điện thoại khác.");
                }
                throw new Exception($"Lỗi khi thêm khách hàng vào database: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra khách hàng có liên quan đến hóa đơn nào không
        /// </summary>
        public async Task<List<OrderInfo>> CheckCustomerOrdersAsync(int customerId)
        {
            List<OrderInfo> orders = new List<OrderInfo>();

            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT 
                            o.order_id,
                            o.order_code,
                            o.total_amount,
                            o.payment_method,
                            o.status,
                            o.created_at,
                            o.updated_at,
                            u.full_name AS user_name
                        FROM Orders o
                        LEFT JOIN Users u ON o.user_id = u.user_id
                        WHERE o.customer_id = @customer_id
                        ORDER BY o.created_at DESC";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@customer_id", customerId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                orders.Add(new OrderInfo
                                {
                                    OrderId = reader.GetInt32("order_id"),
                                    OrderCode = reader.GetString("order_code"),
                                    TotalAmount = reader.GetDecimal("total_amount"),
                                    PaymentMethod = reader.GetString("payment_method"),
                                    Status = reader.GetString("status"),
                                    CreatedAt = reader.GetDateTime("created_at"),
                                    UpdatedAt = reader.IsDBNull("updated_at") ? reader.GetDateTime("created_at") : reader.GetDateTime("updated_at"),
                                    UserName = reader.IsDBNull("user_name") ? "N/A" : reader.GetString("user_name")
                                });
                            }
                        }
                    }
                    return orders;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra hóa đơn của khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa khách hàng khỏi database
        /// </summary>
        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = "DELETE FROM Customers WHERE customer_id = @customer_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@customer_id", customerId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            throw new Exception("Không tìm thấy khách hàng để xóa.");
                        }
                        return true;
                    }
                });
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Lỗi khi xóa khách hàng từ database: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin khách hàng theo ID (bao gồm image BLOB)
        /// </summary>
        public async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT 
                            customer_id,
                            full_name,
                            phone_number,
                            email,
                            address,
                            image,
                            points,
                            created_at,
                            updated_at
                        FROM Customers
                        WHERE customer_id = @customer_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@customer_id", customerId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                Customer customer = new Customer
                                {
                                    CustomerId = reader.GetInt32("customer_id"),
                                    FullName = reader.GetString("full_name"),
                                    PhoneNumber = reader.IsDBNull("phone_number") ? null : reader.GetString("phone_number"),
                                    Email = reader.IsDBNull("email") ? null : reader.GetString("email"),
                                    Address = reader.IsDBNull("address") ? null : reader.GetString("address"),
                                    Points = reader.GetInt32("points"),
                                    CreatedAt = reader.GetDateTime("created_at"),
                                    UpdatedAt = reader.GetDateTime("updated_at")
                                };

                                if (!reader.IsDBNull("image"))
                                {
                                    int imageIndex = reader.GetOrdinal("image");
                                    long imageSize = reader.GetBytes(imageIndex, 0, null, 0, 0);
                                    byte[] imageBytes = new byte[imageSize];
                                    reader.GetBytes(imageIndex, 0, imageBytes, 0, (int)imageSize);
                                    customer.Image = imageBytes;
                                }

                                return customer;
                            }
                            throw new Exception($"Không tìm thấy khách hàng với ID: {customerId}");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng (bao gồm image)
        /// </summary>
        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        UPDATE Customers 
                        SET full_name = @full_name,
                            phone_number = @phone_number,
                            email = @email,
                            address = @address,
                            points = @points";

                    if (customer.Image != null && customer.Image.Length > 0)
                    {
                        query += ", image = @image";
                    }

                    query += " WHERE customer_id = @customer_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@customer_id", customer.CustomerId);
                        command.Parameters.AddWithValue("@full_name", customer.FullName.Trim());
                        command.Parameters.AddWithValue("@phone_number", customer.PhoneNumber.Trim());
                        command.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(customer.Email) ? DBNull.Value : customer.Email.Trim());
                        command.Parameters.AddWithValue("@address", string.IsNullOrWhiteSpace(customer.Address) ? DBNull.Value : customer.Address.Trim());
                        command.Parameters.AddWithValue("@points", customer.Points);

                        if (customer.Image != null && customer.Image.Length > 0)
                        {
                            command.Parameters.AddWithValue("@image", customer.Image);
                        }

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            throw new Exception("Không tìm thấy khách hàng để cập nhật hoặc không có thay đổi nào.");
                        }
                        return true;
                    }
                });
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                {
                    throw new Exception("Số điện thoại này đã được sử dụng bởi khách hàng khác. Vui lòng sử dụng số điện thoại khác.");
                }
                throw new Exception($"Lỗi khi cập nhật khách hàng vào database: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả khách hàng
        /// </summary>
        public async Task<DataTable> LoadCustomersAsync()
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    DataTable dataTable = new DataTable();

                    string query = @"
                        SELECT 
                            customer_id AS 'Mã KH',
                            full_name AS 'Họ tên',
                            phone_number AS 'Số điện thoại',
                            email AS 'Email',
                            address AS 'Địa chỉ',
                            points AS 'Điểm tích lũy',
                            created_at AS 'Ngày tạo',
                            updated_at AS 'Ngày cập nhật'
                        FROM Customers
                        ORDER BY customer_id DESC";

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
                throw new Exception($"Lỗi khi tải danh sách khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm khách hàng theo mã, số điện thoại, email, tên, địa chỉ
        /// </summary>
        public async Task<DataTable> SearchCustomersAsync(string keyword)
        {
            DataTable dataTable = new DataTable();
            string trimmedKeyword = keyword?.Trim() ?? string.Empty;
            bool isEmpty = string.IsNullOrEmpty(trimmedKeyword);

            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT 
                            customer_id AS 'Mã KH',
                            full_name AS 'Họ tên',
                            phone_number AS 'Số điện thoại',
                            email AS 'Email',
                            address AS 'Địa chỉ',
                            points AS 'Điểm tích lũy',
                            created_at AS 'Ngày tạo',
                            updated_at AS 'Ngày cập nhật'
                        FROM Customers
                        WHERE
                            @isEmpty = 1
                            OR CAST(customer_id AS CHAR) LIKE @keyword
                            OR phone_number LIKE @keyword
                            OR email LIKE @keyword
                            OR full_name LIKE @keyword
                            OR address LIKE @keyword
                        ORDER BY customer_id DESC";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@isEmpty", isEmpty ? 1 : 0);
                        command.Parameters.AddWithValue("@keyword", $"%{trimmedKeyword}%");

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
                throw new Exception($"Lỗi khi tìm kiếm khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Đếm tổng số lượng khách hàng
        /// </summary>
        public async Task<int> GetTotalCustomerCountAsync()
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = "SELECT COUNT(*) FROM Customers";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        object? result = await command.ExecuteScalarAsync();
                        return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đếm số lượng khách hàng: {ex.Message}", ex);
            }
        }
    }
}

