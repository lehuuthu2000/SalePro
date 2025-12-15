using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace helloworld
{
    /// <summary>
    /// Data Access Layer cho Products và ProductVariants
    /// Chỉ chứa các thao tác database, không có business logic
    /// </summary>
    internal class ProductDAL
    {
        private ConnectDatabase database;

        public ProductDAL()
        {
            database = new ConnectDatabase();
        }

        /// <summary>
        /// Lấy mã sản phẩm tiếp theo từ database
        /// </summary>
        public async Task<int> GetMaxProductIdAsync()
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = "SELECT MAX(product_id) FROM Products";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        var result = await cmd.ExecuteScalarAsync();
                        if (result != DBNull.Value && result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                        return 0;
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy mã sản phẩm: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin sản phẩm theo ID
        /// </summary>
        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            if (productId <= 0)
                return null;

            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT product_id, product_code, product_name, category_id, description, base_image_path, is_active, created_at, updated_at 
                        FROM Products 
                        WHERE product_id = @id";
                    
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", productId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Product
                                {
                                    ProductId = reader.GetInt32("product_id"),
                                    ProductCode = reader.GetString("product_code"),
                                    ProductName = reader.GetString("product_name"),
                                    CategoryId = reader.IsDBNull("category_id") ? (int?)null : reader.GetInt32("category_id"),
                                    Description = reader.IsDBNull("description") ? null : reader.GetString("description"),
                                    BaseImagePath = reader.IsDBNull("base_image_path") ? null : reader.GetString("base_image_path"),
                                    IsActive = reader.GetBoolean("is_active"),
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
                throw new Exception($"Lỗi khi lấy thông tin sản phẩm: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm sản phẩm mới vào database
        /// </summary>
        public async Task<int> AddProductAsync(string code, string name, int? categoryId, string? desc, string? imgPath, bool isActive)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        INSERT INTO Products (product_code, product_name, category_id, description, base_image_path, is_active, created_at, updated_at)
                        VALUES (@code, @name, @cat, @desc, @img, @active, NOW(), NOW())";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@code", code);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@cat", categoryId.HasValue ? (object)categoryId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@desc", desc ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@img", imgPath ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@active", isActive);

                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = "SELECT LAST_INSERT_ID()";
                        var result = await cmd.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm sản phẩm: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin sản phẩm
        /// </summary>
        public async Task UpdateProductAsync(int productId, string name, int? categoryId, string? desc, string? imgPath, bool isActive)
        {
            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        UPDATE Products 
                        SET product_name = @name, category_id = @cat, description = @desc, base_image_path = @img, is_active = @active, updated_at = NOW()
                        WHERE product_id = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", productId);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@cat", categoryId.HasValue ? (object)categoryId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@desc", desc ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@img", imgPath ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@active", isActive);
                        await cmd.ExecuteNonQueryAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật sản phẩm: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy mã sản phẩm theo ID
        /// </summary>
        public async Task<string> GetProductCodeAsync(int productId)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = "SELECT product_code FROM Products WHERE product_id = @id";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", productId);
                        var result = await cmd.ExecuteScalarAsync();
                        return result?.ToString() ?? string.Empty;
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy mã sản phẩm: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm biến thể sản phẩm
        /// </summary>
        public async Task<int> AddProductVariantAsync(int productId, string sku, string? size, string? color, decimal importPrice, decimal sellingPrice, int stock, string? imgPath, bool isActive)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        INSERT INTO ProductVariants (product_id, sku, size, color, import_price, selling_price, stock_quantity, image_path, is_active, created_at, updated_at)
                        VALUES (@pid, @sku, @size, @color, @iprice, @sprice, @stock, @img, @active, NOW(), NOW())";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@pid", productId);
                        cmd.Parameters.AddWithValue("@sku", sku);
                        cmd.Parameters.AddWithValue("@size", size ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@color", color ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@iprice", importPrice);
                        cmd.Parameters.AddWithValue("@sprice", sellingPrice);
                        cmd.Parameters.AddWithValue("@stock", stock);
                        cmd.Parameters.AddWithValue("@img", imgPath ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@active", isActive);

                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = "SELECT LAST_INSERT_ID()";
                        var result = await cmd.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm biến thể sản phẩm: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin biến thể theo ID
        /// </summary>
        public async Task<ProductVariant?> GetProductVariantByIdAsync(int variantId)
        {
            if (variantId <= 0)
                return null;

            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT variant_id, product_id, sku, size, color, import_price, selling_price, 
                               stock_quantity, total_sales_quantity, total_sales_amount, image_path, is_active
                        FROM ProductVariants
                        WHERE variant_id = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", variantId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new ProductVariant
                                {
                                    VariantId = reader.GetInt32("variant_id"),
                                    ProductId = reader.GetInt32("product_id"),
                                    SKU = reader.GetString("sku"),
                                    Size = reader.IsDBNull("size") ? null : reader.GetString("size"),
                                    Color = reader.IsDBNull("color") ? null : reader.GetString("color"),
                                    ImportPrice = reader.GetDecimal("import_price"),
                                    SellingPrice = reader.GetDecimal("selling_price"),
                                    StockQuantity = reader.GetInt32("stock_quantity"),
                                    TotalSalesQuantity = reader.IsDBNull("total_sales_quantity") ? 0 : reader.GetInt32("total_sales_quantity"),
                                    TotalSalesAmount = reader.IsDBNull("total_sales_amount") ? 0 : reader.GetDecimal("total_sales_amount"),
                                    ImagePath = reader.IsDBNull("image_path") ? null : reader.GetString("image_path"),
                                    IsActive = reader.GetBoolean("is_active")
                                };
                            }
                        }
                    }
                    return null;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin biến thể: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin biến thể
        /// </summary>
        public async Task UpdateProductVariantAsync(int variantId, string sku, string? size, string? color, decimal importPrice, decimal sellingPrice, int stock, string? imgPath, bool isActive)
        {
            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        UPDATE ProductVariants 
                        SET sku = @sku, size = @size, color = @color, import_price = @iprice, 
                            selling_price = @sprice, stock_quantity = @stock, image_path = @img, 
                            is_active = @active, updated_at = NOW()
                        WHERE variant_id = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", variantId);
                        cmd.Parameters.AddWithValue("@sku", sku);
                        cmd.Parameters.AddWithValue("@size", size ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@color", color ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@iprice", importPrice);
                        cmd.Parameters.AddWithValue("@sprice", sellingPrice);
                        cmd.Parameters.AddWithValue("@stock", stock);
                        cmd.Parameters.AddWithValue("@img", imgPath ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@active", isActive);
                        await cmd.ExecuteNonQueryAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật biến thể: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách biến thể của sản phẩm
        /// </summary>
        public async Task<DataTable> LoadProductVariantsAsync(int productId)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT 
                            variant_id AS 'Mã biến thể',
                            sku AS 'SKU',
                            size AS 'Size',
                            color AS 'Màu',
                            import_price AS 'Giá nhập',
                            selling_price AS 'Giá bán',
                            stock_quantity AS 'Số lượng tồn kho',
                            COALESCE(total_sales_quantity, 0) AS 'Số lượng đã bán',
                            COALESCE(total_sales_amount, 0) AS 'Số tiền đã bán',
                            CASE WHEN is_active = 1 THEN 'Đang bán' ELSE 'Ngừng bán' END AS 'Trạng thái'
                        FROM ProductVariants
                        WHERE product_id = @pid
                        ORDER BY variant_id";
                    
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@pid", productId);
                        var dt = new DataTable();
                        using (var adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                        return dt;
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách biến thể: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra các biến thể có trong hóa đơn không
        /// </summary>
        public async Task<Dictionary<int, bool>> CheckVariantsInOrderDetailsAsync(List<int> variantIds)
        {
            if (variantIds == null || variantIds.Count == 0)
                return new Dictionary<int, bool>();

            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    Dictionary<int, bool> result = new Dictionary<int, bool>();

                    if (variantIds.Count == 0)
                        return result;

                    // Tạo danh sách placeholders cho IN clause
                    var placeholders = string.Join(",", variantIds.Select((_, i) => $"@id{i}"));
                    string query = $@"
                        SELECT DISTINCT variant_id 
                        FROM OrderDetails 
                        WHERE variant_id IN ({placeholders})";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        for (int i = 0; i < variantIds.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@id{i}", variantIds[i]);
                        }

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            HashSet<int> variantsInOrders = new HashSet<int>();
                            while (await reader.ReadAsync())
                            {
                                variantsInOrders.Add(reader.GetInt32("variant_id"));
                            }

                            // Tạo dictionary với tất cả variantIds
                            foreach (int variantId in variantIds)
                            {
                                result[variantId] = variantsInOrders.Contains(variantId);
                            }
                        }
                    }
                    return result;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra biến thể trong hóa đơn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa biến thể sản phẩm
        /// </summary>
        public async Task DeleteProductVariantAsync(int variantId)
        {
            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = "DELETE FROM ProductVariants WHERE variant_id = @variant_id";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@variant_id", variantId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa biến thể: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy tổng hợp thông tin sản phẩm (tồn kho, số lượng bán, số tiền bán)
        /// Lưu ý: total_sales_amount và total_sales_quantity trong ProductVariants 
        /// chỉ chứa giá trị từ hóa đơn Completed (đã được xử lý đúng bởi ProcessOrderCompletionAsync)
        /// </summary>
        public async Task<ProductSummary> GetProductSummaryAsync(int productId)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT 
                            COALESCE(SUM(stock_quantity), 0) AS total_stock,
                            COALESCE(SUM(total_sales_quantity), 0) AS total_sales_qty,
                            COALESCE(SUM(total_sales_amount), 0) AS total_sales_amt
                        FROM ProductVariants
                        WHERE product_id = @pid";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@pid", productId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new ProductSummary
                                {
                                    TotalStockQuantity = reader.GetInt32("total_stock"),
                                    TotalSalesQuantity = reader.GetInt32("total_sales_qty"),
                                    TotalSalesAmount = reader.GetDecimal("total_sales_amt")
                                };
                            }
                        }
                    }
                    return new ProductSummary();
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy tổng hợp sản phẩm: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách sản phẩm
        /// </summary>
        public async Task<DataTable> LoadProductsAsync()
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT 
                            p.product_id AS 'Mã SP',
                            p.product_code AS 'Mã Hiển Thị',
                            p.product_name AS 'Tên Sản Phẩm',
                            c.category_name AS 'Danh Mục',
                            COUNT(v.variant_id) AS 'Số biến thể',
                            COALESCE(SUM(v.stock_quantity), 0) AS 'Tổng tồn kho',
                            MIN(v.selling_price) AS 'Giá thấp nhất',
                            MAX(v.selling_price) AS 'Giá cao nhất',
                            COALESCE(SUM(v.total_sales_quantity), 0) AS 'Tổng số lượng bán',
                            COALESCE(SUM(v.total_sales_amount), 0) AS 'Tổng số tiền bán',
                            CASE WHEN p.is_active = 1 THEN 'Đang Bán' ELSE 'Ngừng Bán' END AS 'Trạng Thái'
                        FROM Products p
                        LEFT JOIN Categories c ON p.category_id = c.category_id
                        LEFT JOIN ProductVariants v ON p.product_id = v.product_id
                        GROUP BY p.product_id, p.product_code, p.product_name, c.category_name, p.is_active, p.created_at
                        ORDER BY p.created_at DESC";
                    
                    var dt = new DataTable();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        using(var adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                    return dt;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách sản phẩm: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm sản phẩm
        /// </summary>
        public async Task<DataTable> SearchProductsAsync(string keyword)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT 
                            p.product_id AS 'Mã SP',
                            p.product_code AS 'Mã Hiển Thị',
                            p.product_name AS 'Tên Sản Phẩm',
                            c.category_name AS 'Danh Mục',
                            COUNT(v.variant_id) AS 'Số biến thể',
                            COALESCE(SUM(v.stock_quantity), 0) AS 'Tổng tồn kho',
                            MIN(v.selling_price) AS 'Giá thấp nhất',
                            MAX(v.selling_price) AS 'Giá cao nhất',
                            COALESCE(SUM(v.total_sales_quantity), 0) AS 'Tổng số lượng bán',
                            COALESCE(SUM(v.total_sales_amount), 0) AS 'Tổng số tiền bán',
                            CASE WHEN p.is_active = 1 THEN 'Đang Bán' ELSE 'Ngừng Bán' END AS 'Trạng Thái'
                        FROM Products p
                        LEFT JOIN Categories c ON p.category_id = c.category_id
                        LEFT JOIN ProductVariants v ON p.product_id = v.product_id
                        WHERE p.product_name LIKE @kw OR p.product_code LIKE @kw OR c.category_name LIKE @kw
                        GROUP BY p.product_id, p.product_code, p.product_name, c.category_name, p.is_active, p.created_at
                        ORDER BY p.created_at DESC";
                    
                    var dt = new DataTable();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                        using(var adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                    return dt;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm sản phẩm: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách OrderDetails liên quan đến sản phẩm
        /// </summary>
        public async Task<List<RelatedOrderDetail>> GetRelatedOrderDetailsAsync(int productId)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    List<RelatedOrderDetail> orderDetails = new List<RelatedOrderDetail>();

                    string query = @"
                        SELECT 
                            o.order_code,
                            pv.sku,
                            od.quantity
                        FROM OrderDetails od
                        INNER JOIN ProductVariants pv ON od.variant_id = pv.variant_id
                        INNER JOIN Orders o ON od.order_id = o.order_id
                        WHERE pv.product_id = @product_id
                        ORDER BY o.order_code, pv.sku
                        LIMIT 10";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@product_id", productId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                orderDetails.Add(new RelatedOrderDetail
                                {
                                    OrderCode = reader.GetString("order_code"),
                                    VariantSKU = reader.GetString("sku"),
                                    Quantity = reader.GetInt32("quantity")
                                });
                            }
                        }
                    }
                    return orderDetails;
                });
            }
            catch
            {
                return new List<RelatedOrderDetail>();
            }
        }

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        public async Task DeleteProductAsync(int productId)
        {
            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = "DELETE FROM Products WHERE product_id = @id";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", productId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa sản phẩm: {ex.Message}", ex);
            }
        }
    }
}

