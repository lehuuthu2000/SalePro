using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace helloworld
{
    /// <summary>
    /// Data Access Layer cho Orders và OrderDetails
    /// Chỉ chứa các thao tác database, không có business logic
    /// </summary>
    internal class OrderDAL
    {
        private ConnectDatabase database;

        public OrderDAL()
        {
            database = new ConnectDatabase();
        }

        /// <summary>
        /// Lấy danh sách hóa đơn từ bảng Orders
        /// </summary>
        public async Task<DataTable> LoadOrdersAsync()
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    DataTable dataTable = new DataTable();

                    string query = @"
                        SELECT 
                            order_id AS 'Mã HĐ',
                            order_code AS 'Mã hóa đơn',
                            customer_id AS 'Mã KH',
                            user_id AS 'Mã NV',
                            total_amount AS 'Tổng tiền',
                            discount_amount AS 'Giảm giá',
                            status AS 'Trạng thái'
                        FROM Orders
                        ORDER BY created_at DESC";

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
                throw new Exception($"Lỗi khi tải danh sách hóa đơn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy số lượng hóa đơn trong ngày để tạo mã hóa đơn
        /// </summary>
        public async Task<int> GetTodayOrderCountAsync()
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT COUNT(*) 
                        FROM Orders 
                        WHERE DATE(created_at) = CURDATE()";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        object? result = await command.ExecuteScalarAsync();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy số lượng hóa đơn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin khách hàng theo ID
        /// </summary>
        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            if (customerId <= 0)
                return null;

            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT customer_id, full_name, phone_number, email, address, points
                        FROM Customers
                        WHERE customer_id = @customer_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@customer_id", customerId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Customer
                                {
                                    CustomerId = reader.GetInt32("customer_id"),
                                    FullName = reader.GetString("full_name"),
                                    PhoneNumber = reader.IsDBNull("phone_number") ? null : reader.GetString("phone_number"),
                                    Email = reader.IsDBNull("email") ? null : reader.GetString("email"),
                                    Address = reader.IsDBNull("address") ? null : reader.GetString("address"),
                                    Points = reader.IsDBNull("points") ? 0 : reader.GetInt32("points")
                                };
                            }
                        }
                    }
                    return null;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin khách hàng theo số điện thoại
        /// </summary>
        public async Task<Customer?> GetCustomerByPhoneAsync(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return null;

            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT customer_id, full_name, phone_number, email, address, points
                        FROM Customers
                        WHERE phone_number = @phone_number";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@phone_number", phoneNumber.Trim());

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Customer
                                {
                                    CustomerId = reader.GetInt32("customer_id"),
                                    FullName = reader.GetString("full_name"),
                                    PhoneNumber = reader.IsDBNull("phone_number") ? null : reader.GetString("phone_number"),
                                    Email = reader.IsDBNull("email") ? null : reader.GetString("email"),
                                    Address = reader.IsDBNull("address") ? null : reader.GetString("address"),
                                    Points = reader.IsDBNull("points") ? 0 : reader.GetInt32("points")
                                };
                            }
                        }
                    }
                    return null;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm khách hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin variant theo ID (không kiểm tra is_active - dùng cho load hóa đơn cũ)
        /// </summary>
        public async Task<ProductVariantInfo?> GetVariantInfoByIdAsync(int variantId)
        {
            if (variantId <= 0)
            {
                return null;
            }

            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT 
                            pv.variant_id,
                            pv.sku,
                            p.product_name,
                            pv.size,
                            pv.color,
                            pv.selling_price,
                            pv.stock_quantity
                        FROM ProductVariants pv
                        INNER JOIN Products p ON pv.product_id = p.product_id
                        WHERE pv.variant_id = @variant_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@variant_id", variantId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new ProductVariantInfo
                                {
                                    VariantId = reader.GetInt32("variant_id"),
                                    SKU = reader.GetString("sku"),
                                    ProductName = reader.GetString("product_name"),
                                    Size = reader.IsDBNull("size") ? null : reader.GetString("size"),
                                    Color = reader.IsDBNull("color") ? null : reader.GetString("color"),
                                    SellingPrice = reader.GetDecimal("selling_price"),
                                    StockQuantity = reader.GetInt32("stock_quantity")
                                };
                            }
                        }
                    }
                    return null;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin variant (VariantId: {variantId}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách ProductVariants có sẵn để bán
        /// </summary>
        public async Task<List<ProductVariantInfo>> GetProductVariantsAsync()
        {
            List<ProductVariantInfo> variants = new List<ProductVariantInfo>();

            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT 
                            pv.variant_id,
                            pv.sku,
                            p.product_name,
                            pv.size,
                            pv.color,
                            pv.selling_price,
                            pv.stock_quantity
                        FROM ProductVariants pv
                        INNER JOIN Products p ON pv.product_id = p.product_id
                        WHERE pv.is_active = TRUE AND p.is_active = TRUE
                        ORDER BY p.product_name, pv.size, pv.color";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                variants.Add(new ProductVariantInfo
                                {
                                    VariantId = reader.GetInt32("variant_id"),
                                    SKU = reader.GetString("sku"),
                                    ProductName = reader.GetString("product_name"),
                                    Size = reader.IsDBNull("size") ? null : reader.GetString("size"),
                                    Color = reader.IsDBNull("color") ? null : reader.GetString("color"),
                                    SellingPrice = reader.GetDecimal("selling_price"),
                                    StockQuantity = reader.GetInt32("stock_quantity")
                                });
                            }
                        }
                    }
                    return variants;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách sản phẩm: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm hóa đơn mới vào database
        /// </summary>
        public async Task<int> AddOrderAsync(Order order)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        INSERT INTO Orders (
                            order_code, customer_id, user_id, tax, total_amount, 
                            discount_amount, payment_method, status, 
                            shipping_address, billing_address, note
                        )
                        VALUES (
                            @order_code, @customer_id, @user_id, @tax, @total_amount,
                            @discount_amount, @payment_method, @status,
                            @shipping_address, @billing_address, @note
                        )";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@order_code", order.OrderCode);
                        command.Parameters.AddWithValue("@customer_id", order.CustomerId.HasValue ? (object)order.CustomerId.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@user_id", order.UserId);
                        command.Parameters.AddWithValue("@tax", order.Tax);
                        command.Parameters.AddWithValue("@total_amount", order.TotalAmount);
                        command.Parameters.AddWithValue("@discount_amount", order.DiscountAmount);
                        command.Parameters.AddWithValue("@payment_method", order.PaymentMethod);
                        command.Parameters.AddWithValue("@status", order.Status);
                        command.Parameters.AddWithValue("@shipping_address", string.IsNullOrWhiteSpace(order.ShippingAddress) ? DBNull.Value : order.ShippingAddress);
                        command.Parameters.AddWithValue("@billing_address", string.IsNullOrWhiteSpace(order.BillingAddress) ? DBNull.Value : order.BillingAddress);
                        command.Parameters.AddWithValue("@note", string.IsNullOrWhiteSpace(order.Note) ? DBNull.Value : order.Note);

                        await command.ExecuteNonQueryAsync();

                        command.CommandText = "SELECT LAST_INSERT_ID()";
                        object? result = await command.ExecuteScalarAsync();

                        if (result != null && int.TryParse(result.ToString(), out int orderId))
                        {
                            return orderId;
                        }
                        throw new Exception("Không thể lấy Order ID sau khi thêm hóa đơn.");
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm hóa đơn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm chi tiết hóa đơn vào database
        /// </summary>
        public async Task AddOrderDetailAsync(OrderDetail orderDetail)
        {
            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        INSERT INTO OrderDetails (order_id, variant_id, quantity, unit_price, subtotal)
                        VALUES (@order_id, @variant_id, @quantity, @unit_price, @subtotal)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@order_id", orderDetail.OrderId);
                        command.Parameters.AddWithValue("@variant_id", orderDetail.VariantId);
                        command.Parameters.AddWithValue("@quantity", orderDetail.Quantity);
                        command.Parameters.AddWithValue("@unit_price", orderDetail.UnitPrice);
                        command.Parameters.AddWithValue("@subtotal", orderDetail.Subtotal);

                        await command.ExecuteNonQueryAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm chi tiết hóa đơn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm nhiều chi tiết hóa đơn cùng lúc (transaction)
        /// </summary>
        public async Task AddOrderDetailsAsync(List<OrderDetail> orderDetails)
        {
            if (orderDetails == null || orderDetails.Count == 0)
            {
                return;
            }

            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            string query = @"
                                INSERT INTO OrderDetails (order_id, variant_id, quantity, unit_price, subtotal)
                                VALUES (@order_id, @variant_id, @quantity, @unit_price, @subtotal)";

                            foreach (var detail in orderDetails)
                            {
                                using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@order_id", detail.OrderId);
                                    command.Parameters.AddWithValue("@variant_id", detail.VariantId);
                                    command.Parameters.AddWithValue("@quantity", detail.Quantity);
                                    command.Parameters.AddWithValue("@unit_price", detail.UnitPrice);
                                    command.Parameters.AddWithValue("@subtotal", detail.Subtotal);

                                    await command.ExecuteNonQueryAsync();
                                }
                            }

                            await transaction.CommitAsync();
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm chi tiết hóa đơn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin hóa đơn theo Order ID
        /// </summary>
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            if (orderId <= 0)
            {
                return null;
            }

            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT 
                            order_id, order_code, customer_id, user_id, tax, 
                            total_amount, discount_amount, payment_method, status,
                            shipping_address, billing_address, note, created_at, updated_at
                        FROM Orders
                        WHERE order_id = @order_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@order_id", orderId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Order
                                {
                                    OrderId = reader.GetInt32("order_id"),
                                    OrderCode = reader.GetString("order_code"),
                                    CustomerId = reader.IsDBNull("customer_id") ? null : reader.GetInt32("customer_id"),
                                    UserId = reader.GetInt32("user_id"),
                                    Tax = reader.GetDecimal("tax"),
                                    TotalAmount = reader.GetDecimal("total_amount"),
                                    DiscountAmount = reader.GetDecimal("discount_amount"),
                                    PaymentMethod = reader.GetString("payment_method"),
                                    Status = reader.GetString("status"),
                                    ShippingAddress = reader.IsDBNull("shipping_address") ? null : reader.GetString("shipping_address"),
                                    BillingAddress = reader.IsDBNull("billing_address") ? null : reader.GetString("billing_address"),
                                    Note = reader.IsDBNull("note") ? null : reader.GetString("note")
                                };
                            }
                        }
                    }
                    return null;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin hóa đơn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin hóa đơn kèm ngày tạo và ngày cập nhật
        /// </summary>
        public async Task<OrderWithDates?> GetOrderWithDatesAsync(int orderId)
        {
            if (orderId <= 0)
            {
                return null;
            }

            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT created_at, updated_at
                        FROM Orders
                        WHERE order_id = @order_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@order_id", orderId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new OrderWithDates
                                {
                                    CreatedAt = reader.GetDateTime("created_at"),
                                    UpdatedAt = reader.GetDateTime("updated_at")
                                };
                            }
                        }
                    }
                    return null;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy ngày tạo/cập nhật hóa đơn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách chi tiết hóa đơn theo Order ID
        /// </summary>
        public async Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            List<OrderDetail> orderDetails = new List<OrderDetail>();

            if (orderId <= 0)
            {
                return orderDetails;
            }

            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT 
                            od.order_detail_id,
                            od.order_id,
                            od.variant_id,
                            od.quantity,
                            od.unit_price,
                            od.subtotal
                        FROM OrderDetails od
                        WHERE od.order_id = @order_id
                        ORDER BY od.order_detail_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@order_id", orderId);
                        command.CommandTimeout = 30;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                orderDetails.Add(new OrderDetail
                                {
                                    OrderDetailId = reader.GetInt32("order_detail_id"),
                                    OrderId = reader.GetInt32("order_id"),
                                    VariantId = reader.GetInt32("variant_id"),
                                    Quantity = reader.GetInt32("quantity"),
                                    UnitPrice = reader.GetDecimal("unit_price"),
                                    Subtotal = reader.GetDecimal("subtotal")
                                });
                            }
                        }
                    }
                    return orderDetails;
                });
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Lỗi MySQL khi lấy chi tiết hóa đơn (OrderId: {orderId}): {ex.Message} (Error Code: {ex.Number})", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy chi tiết hóa đơn (OrderId: {orderId}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật số lượng tồn kho của variant (giảm khi bán)
        /// Đảm bảo không bị âm khi trừ đi
        /// </summary>
        public async Task UpdateVariantStockAsync(int variantId, int quantity)
        {
            if (variantId <= 0)
            {
                return;
            }

            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        UPDATE ProductVariants 
                        SET stock_quantity = GREATEST(0, stock_quantity + @quantity),
                            updated_at = CURRENT_TIMESTAMP
                        WHERE variant_id = @variant_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@variant_id", variantId);
                        command.Parameters.AddWithValue("@quantity", quantity);
                        await command.ExecuteNonQueryAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật tồn kho (VariantId: {variantId}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật số tiền bán và số lượng bán của variant
        /// Đảm bảo không bị âm khi trừ đi
        /// </summary>
        public async Task UpdateVariantSalesAsync(int variantId, int quantity, decimal amount)
        {
            if (variantId <= 0)
            {
                return;
            }

            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        UPDATE ProductVariants 
                        SET 
                            total_sales_quantity = GREATEST(0, COALESCE(total_sales_quantity, 0) + @quantity),
                            total_sales_amount = GREATEST(0, COALESCE(total_sales_amount, 0) + @amount),
                            updated_at = CURRENT_TIMESTAMP
                        WHERE variant_id = @variant_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@variant_id", variantId);
                        command.Parameters.AddWithValue("@quantity", quantity);
                        command.Parameters.AddWithValue("@amount", amount);
                        await command.ExecuteNonQueryAsync();
                    }
                });
            }
            catch (MySqlException ex) when (ex.Number == 1054) // Unknown column
            {
                // Nếu cột chưa tồn tại, bỏ qua (người dùng cần chạy script SQL trước)
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật số liệu bán hàng (VariantId: {variantId}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy trạng thái cũ của hóa đơn
        /// </summary>
        public async Task<string?> GetOldOrderStatusAsync(int orderId)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = "SELECT status FROM Orders WHERE order_id = @order_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@order_id", orderId);
                        object? result = await command.ExecuteScalarAsync();
                        return result?.ToString();
                    }
                });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Cập nhật thông tin hóa đơn
        /// </summary>
        public async Task UpdateOrderAsync(Order order)
        {
            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        UPDATE Orders SET
                            order_code = @order_code,
                            customer_id = @customer_id,
                            user_id = @user_id,
                            tax = @tax,
                            total_amount = @total_amount,
                            discount_amount = @discount_amount,
                            payment_method = @payment_method,
                            status = @status,
                            shipping_address = @shipping_address,
                            billing_address = @billing_address,
                            note = @note,
                            updated_at = CURRENT_TIMESTAMP
                        WHERE order_id = @order_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@order_id", order.OrderId);
                        command.Parameters.AddWithValue("@order_code", order.OrderCode);
                        command.Parameters.AddWithValue("@customer_id", order.CustomerId.HasValue ? (object)order.CustomerId.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@user_id", order.UserId);
                        command.Parameters.AddWithValue("@tax", order.Tax);
                        command.Parameters.AddWithValue("@total_amount", order.TotalAmount);
                        command.Parameters.AddWithValue("@discount_amount", order.DiscountAmount);
                        command.Parameters.AddWithValue("@payment_method", order.PaymentMethod);
                        command.Parameters.AddWithValue("@status", order.Status);
                        command.Parameters.AddWithValue("@shipping_address", string.IsNullOrWhiteSpace(order.ShippingAddress) ? DBNull.Value : order.ShippingAddress);
                        command.Parameters.AddWithValue("@billing_address", string.IsNullOrWhiteSpace(order.BillingAddress) ? DBNull.Value : order.BillingAddress);
                        command.Parameters.AddWithValue("@note", string.IsNullOrWhiteSpace(order.Note) ? DBNull.Value : order.Note);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            throw new Exception("Không tìm thấy hóa đơn để cập nhật.");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật hóa đơn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa tất cả chi tiết hóa đơn theo Order ID
        /// </summary>
        public async Task DeleteOrderDetailsAsync(int orderId)
        {
            if (orderId <= 0)
            {
                return;
            }

            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = "DELETE FROM OrderDetails WHERE order_id = @order_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@order_id", orderId);
                        await command.ExecuteNonQueryAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa chi tiết hóa đơn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa hóa đơn (xóa cả OrderDetails và Order)
        /// </summary>
        public async Task DeleteOrderAsync(int orderId)
        {
            if (orderId <= 0)
            {
                throw new ArgumentException("Order ID không hợp lệ.");
            }

            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    // Sử dụng transaction để đảm bảo tính toàn vẹn dữ liệu
                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            // Xóa OrderDetails trước (do foreign key constraint)
                            string deleteDetailsQuery = "DELETE FROM OrderDetails WHERE order_id = @order_id";
                            using (MySqlCommand command = new MySqlCommand(deleteDetailsQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@order_id", orderId);
                                await command.ExecuteNonQueryAsync();
                            }

                            // Xóa Order
                            string deleteOrderQuery = "DELETE FROM Orders WHERE order_id = @order_id";
                            using (MySqlCommand command = new MySqlCommand(deleteOrderQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@order_id", orderId);
                                int rowsAffected = await command.ExecuteNonQueryAsync();
                                
                                if (rowsAffected == 0)
                                {
                                    throw new Exception("Không tìm thấy hóa đơn để xóa.");
                                }
                            }

                            // Commit transaction
                            await transaction.CommitAsync();
                        }
                        catch
                        {
                            // Rollback nếu có lỗi
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa hóa đơn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tính tổng doanh số từ các hóa đơn đã thanh toán (status = 'Completed')
        /// </summary>
        public async Task<decimal> GetTotalRevenueAsync()
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT COALESCE(SUM(total_amount), 0) AS total_revenue
                        FROM Orders
                        WHERE status = 'Completed'";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        object? result = await command.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToDecimal(result);
                        }
                    }
                    return 0;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tính tổng doanh số: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tính lại số tiền bán và số lượng bán của variant dựa trên tất cả hóa đơn Completed
        /// Đồng thời tính lại tồn kho
        /// </summary>
        public async Task RecalculateVariantSalesAsync(int variantId)
        {
            if (variantId <= 0)
            {
                return;
            }

            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    // Tính tổng từ tất cả OrderDetails có status = 'Completed'
                    // Tính lại stock_quantity = stock_quantity_hiện_tại + (total_sales_quantity_cũ - total_sales_quantity_mới)
                    // Vì stock_quantity đã được giảm khi bán, nên cần cộng lại phần chênh lệch
                    string query = @"
                        UPDATE ProductVariants pv
                        SET 
                            stock_quantity = GREATEST(0, 
                                pv.stock_quantity + COALESCE(pv.total_sales_quantity, 0) - 
                                COALESCE((
                                    SELECT SUM(od.quantity)
                                    FROM OrderDetails od
                                    INNER JOIN Orders o ON od.order_id = o.order_id
                                    WHERE od.variant_id = pv.variant_id AND o.status = 'Completed'
                                ), 0)
                            ),
                            total_sales_quantity = COALESCE((
                                SELECT SUM(od.quantity)
                                FROM OrderDetails od
                                INNER JOIN Orders o ON od.order_id = o.order_id
                                WHERE od.variant_id = pv.variant_id AND o.status = 'Completed'
                            ), 0),
                            total_sales_amount = COALESCE((
                                SELECT SUM(od.subtotal)
                                FROM OrderDetails od
                                INNER JOIN Orders o ON od.order_id = o.order_id
                                WHERE od.variant_id = pv.variant_id AND o.status = 'Completed'
                            ), 0),
                            updated_at = CURRENT_TIMESTAMP
                        WHERE pv.variant_id = @variant_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@variant_id", variantId);
                        await command.ExecuteNonQueryAsync();
                    }
                });
            }
            catch (MySqlException ex) when (ex.Number == 1054) // Unknown column
            {
                // Nếu cột chưa tồn tại, bỏ qua
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tính lại số liệu bán hàng (VariantId: {variantId}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tính lại số tiền bán và số lượng bán của tất cả variants trong một sản phẩm
        /// Đồng thời tính lại tồn kho dựa trên số lượng đã bán trong hóa đơn Completed
        /// </summary>
        public async Task RecalculateProductSalesAsync(int productId)
        {
            if (productId <= 0)
            {
                return;
            }

            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    // Tính lại cho tất cả variants của sản phẩm trong một query
                    // Tính lại stock_quantity = stock_quantity_hiện_tại + (total_sales_quantity_cũ - total_sales_quantity_mới)
                    // Vì stock_quantity đã được giảm khi bán, nên cần cộng lại phần chênh lệch
                    string query = @"
                        UPDATE ProductVariants pv
                        SET 
                            stock_quantity = GREATEST(0, 
                                pv.stock_quantity + COALESCE(pv.total_sales_quantity, 0) - 
                                COALESCE((
                                    SELECT SUM(od.quantity)
                                    FROM OrderDetails od
                                    INNER JOIN Orders o ON od.order_id = o.order_id
                                    WHERE od.variant_id = pv.variant_id AND o.status = 'Completed'
                                ), 0)
                            ),
                            total_sales_quantity = COALESCE((
                                SELECT SUM(od.quantity)
                                FROM OrderDetails od
                                INNER JOIN Orders o ON od.order_id = o.order_id
                                WHERE od.variant_id = pv.variant_id AND o.status = 'Completed'
                            ), 0),
                            total_sales_amount = COALESCE((
                                SELECT SUM(od.subtotal)
                                FROM OrderDetails od
                                INNER JOIN Orders o ON od.order_id = o.order_id
                                WHERE od.variant_id = pv.variant_id AND o.status = 'Completed'
                            ), 0),
                            updated_at = CURRENT_TIMESTAMP
                        WHERE pv.product_id = @product_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@product_id", productId);
                        await command.ExecuteNonQueryAsync();
                    }
                });
            }
            catch (MySqlException ex) when (ex.Number == 1054) // Unknown column
            {
                // Nếu cột chưa tồn tại, bỏ qua
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tính lại số liệu bán hàng cho sản phẩm (ProductId: {productId}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tính lại tồn kho của variant dựa trên số lượng đã bán trong hóa đơn Completed
        /// stock_quantity = stock_quantity_ban_dau - tổng_số_lượng_đã_bán_trong_hóa_đơn_Completed
        /// </summary>
        public async Task RecalculateVariantStockAsync(int variantId)
        {
            if (variantId <= 0)
            {
                return;
            }

            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    // Tính lại stock_quantity dựa trên: stock_quantity_hiện_tại + tổng_số_lượng_đã_bán - tổng_số_lượng_đã_bán_trong_hóa_đơn_Completed
                    // Vì stock_quantity_hiện_tại đã được giảm khi bán, nên cần cộng lại số lượng đã bán để có stock_ban_dau
                    string query = @"
                        UPDATE ProductVariants pv
                        SET 
                            stock_quantity = GREATEST(0, 
                                pv.stock_quantity + COALESCE(pv.total_sales_quantity, 0) - 
                                COALESCE((
                                    SELECT SUM(od.quantity)
                                    FROM OrderDetails od
                                    INNER JOIN Orders o ON od.order_id = o.order_id
                                    WHERE od.variant_id = pv.variant_id AND o.status = 'Completed'
                                ), 0)
                            ),
                            updated_at = CURRENT_TIMESTAMP
                        WHERE pv.variant_id = @variant_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@variant_id", variantId);
                        await command.ExecuteNonQueryAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tính lại tồn kho (VariantId: {variantId}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách hóa đơn gần nhất
        /// </summary>
        public async Task<List<OrderInfo>> GetRecentOrdersAsync(int limit = 5)
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
                            COALESCE(c.full_name, 'Khách vãng lai') AS customer_name,
                            u.full_name AS user_name
                        FROM Orders o
                        LEFT JOIN Customers c ON o.customer_id = c.customer_id
                        LEFT JOIN Users u ON o.user_id = u.user_id
                        ORDER BY o.created_at DESC
                        LIMIT @limit";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@limit", limit);

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
                                    CustomerName = reader.IsDBNull("customer_name") ? null : reader.GetString("customer_name"),
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
                throw new Exception($"Lỗi khi lấy danh sách hóa đơn gần nhất: {ex.Message}", ex);
            }
        }
    }
}

