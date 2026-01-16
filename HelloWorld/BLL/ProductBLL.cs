using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace helloworld
{
    /// <summary>
    /// Business Logic Layer cho Products và ProductVariants
    /// Chứa business logic, validation, và gọi ProductDAL
    /// Giữ nguyên interface public như SanPhamModels cũ để tương thích với Forms
    /// </summary>
    internal class ProductBLL
    {
        private ProductDAL productDAL;
        private CategoryDAL categoryDAL;

        public ProductBLL()
        {
            productDAL = new ProductDAL();
            categoryDAL = new CategoryDAL();
        }

        /// <summary>
        /// Tạo mã sản phẩm tự động
        /// </summary>
        public async Task<string> GenerateProductCodeAsync()
        {
            try
            {
                int maxId = await productDAL.GetMaxProductIdAsync();
                int nextId = maxId + 1;
                return $"SP{nextId:D6}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo mã sản phẩm: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin sản phẩm theo ID
        /// </summary>
        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await productDAL.GetProductByIdAsync(productId);
        }

        /// <summary>
        /// Lấy danh sách danh mục
        /// </summary>
        public async Task<List<Category>> LoadCategoriesAsync()
        {
            return await categoryDAL.GetCategoriesListAsync();
        }

        /// <summary>
        /// Thêm sản phẩm mới
        /// </summary>
        public async Task<int> AddProductAsync(string code, string name, int? categoryId, string? desc, string? imgPath, bool isActive)
        {
            return await productDAL.AddProductAsync(code, name, categoryId, desc, imgPath, isActive);
        }

        /// <summary>
        /// Cập nhật thông tin sản phẩm
        /// </summary>
        public async Task UpdateProductAsync(int productId, string name, int? categoryId, string? desc, string? imgPath, bool isActive)
        {
            await productDAL.UpdateProductAsync(productId, name, categoryId, desc, imgPath, isActive);
        }

        /// <summary>
        /// Lấy mã sản phẩm theo ID
        /// </summary>
        public async Task<string> GetProductCodeAsync(int productId)
        {
            return await productDAL.GetProductCodeAsync(productId);
        }

        /// <summary>
        /// Thêm biến thể sản phẩm
        /// </summary>
        public async Task<int> AddProductVariantAsync(int productId, string sku, string? size, string? color, decimal importPrice, decimal sellingPrice, int stock, string? imgPath, bool isActive)
        {
            return await productDAL.AddProductVariantAsync(productId, sku, size, color, importPrice, sellingPrice, stock, imgPath, isActive);
        }

        /// <summary>
        /// Lấy thông tin biến thể theo ID
        /// </summary>
        public async Task<ProductVariant?> GetProductVariantByIdAsync(int variantId)
        {
            return await productDAL.GetProductVariantByIdAsync(variantId);
        }

        /// <summary>
        /// Cập nhật thông tin biến thể
        /// </summary>
        public async Task UpdateProductVariantAsync(int variantId, string sku, string? size, string? color, decimal importPrice, decimal sellingPrice, int stock, string? imgPath, bool isActive)
        {
            await productDAL.UpdateProductVariantAsync(variantId, sku, size, color, importPrice, sellingPrice, stock, imgPath, isActive);
        }

        /// <summary>
        /// Lấy danh sách biến thể của sản phẩm
        /// </summary>
        public async Task<DataTable> LoadProductVariantsAsync(int productId)
        {
            return await productDAL.LoadProductVariantsAsync(productId);
        }

        /// <summary>
        /// Kiểm tra các biến thể có trong hóa đơn không
        /// </summary>
        public async Task<Dictionary<int, bool>> CheckVariantsInOrderDetailsAsync(List<int> variantIds)
        {
            return await productDAL.CheckVariantsInOrderDetailsAsync(variantIds);
        }

        /// <summary>
        /// Xóa biến thể sản phẩm (kiểm tra hóa đơn trước)
        /// </summary>
        public async Task<DeleteVariantResult> DeleteProductVariantsAsync(List<int> variantIds)
        {
            DeleteVariantResult result = new DeleteVariantResult();
            
            if (variantIds == null || variantIds.Count == 0)
                return result;

            try
            {
                // Lấy thông tin SKU cho các variant
                Dictionary<int, string> variantSKUs = new Dictionary<int, string>();
                foreach (int id in variantIds)
                {
                    var variant = await GetProductVariantByIdAsync(id);
                    if (variant != null)
                    {
                        variantSKUs[id] = variant.SKU;
                    }
                    else
                    {
                        variantSKUs[id] = $"Variant ID: {id}";
                    }
                }

                // Kiểm tra tất cả variants cùng lúc xem có trong hóa đơn không
                Dictionary<int, bool> variantsInOrdersCheck = await CheckVariantsInOrderDetailsAsync(variantIds);

                // Thực hiện xóa từng variant
                foreach (int variantId in variantIds)
                {
                    string currentSku = variantSKUs.TryGetValue(variantId, out string? sku) ? sku : $"Variant ID: {variantId}";

                    try
                    {
                        if (variantsInOrdersCheck.TryGetValue(variantId, out bool isInOrder) && isInOrder)
                        {
                            result.FailedVariants.Add(new FailedDeleteVariantInfo
                            {
                                VariantId = variantId,
                                SKU = currentSku,
                                Reason = "Có trong hóa đơn liên quan"
                            });
                            continue;
                        }

                        // Thực hiện xóa
                        await productDAL.DeleteProductVariantAsync(variantId);

                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        result.FailedVariants.Add(new FailedDeleteVariantInfo
                        {
                            VariantId = variantId,
                            SKU = currentSku,
                            Reason = ex.Message
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa biến thể sản phẩm: {ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// Lấy tổng hợp thông tin sản phẩm (tồn kho, số lượng bán, số tiền bán)
        /// </summary>
        public async Task<ProductSummary> GetProductSummaryAsync(int productId)
        {
            return await productDAL.GetProductSummaryAsync(productId);
        }

        /// <summary>
        /// Tính lại số tiền bán và số lượng bán của tất cả variants trong một sản phẩm
        /// Dựa trên tất cả hóa đơn Completed để đảm bảo chính xác
        /// </summary>
        public async Task RecalculateProductSalesAsync(int productId)
        {
            // Sử dụng OrderDAL để tính lại
            OrderBLL orderBLL = new OrderBLL();
            await orderBLL.RecalculateProductSalesAsync(productId);
        }

        /// <summary>
        /// Lấy danh sách sản phẩm
        /// </summary>
        public async Task<DataTable> LoadProductsAsync()
        {
            return await productDAL.LoadProductsAsync();
        }

        /// <summary>
        /// Tìm kiếm sản phẩm
        /// </summary>
        public async Task<DataTable> SearchProductsAsync(string keyword)
        {
            return await productDAL.SearchProductsAsync(keyword);
        }

        /// <summary>
        /// Xóa sản phẩm (kiểm tra hóa đơn trước)
        /// </summary>
        public async Task<DeleteProductResult> DeleteProductsAsync(List<int> productIds)
        {
            DeleteProductResult result = new DeleteProductResult();
            
            if (productIds == null || productIds.Count == 0)
                return result;

            try
            {
                foreach (var productId in productIds)
                {
                    try
                    {
                        // Lấy tên sản phẩm
                        var product = await GetProductByIdAsync(productId);
                        string productName = product?.ProductName ?? $"Product ID: {productId}";

                        // Kiểm tra có biến thể trong hóa đơn không
                        var relatedOrderDetails = await productDAL.GetRelatedOrderDetailsAsync(productId);
                        
                        if (relatedOrderDetails.Count > 0)
                        {
                            result.FailedProducts.Add(new FailedDeleteProductInfo
                            {
                                ProductId = productId,
                                ProductName = productName,
                                Reason = "Có biến thể trong hóa đơn liên quan",
                                RelatedOrderDetails = relatedOrderDetails
                            });
                            continue;
                        }

                        // Thực hiện xóa (CASCADE sẽ xóa các variants)
                        await productDAL.DeleteProductAsync(productId);

                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        var product = await GetProductByIdAsync(productId);
                        var relatedOrderDetails = await productDAL.GetRelatedOrderDetailsAsync(productId);
                        
                        result.FailedProducts.Add(new FailedDeleteProductInfo
                        {
                            ProductId = productId,
                            ProductName = product?.ProductName ?? $"Product ID: {productId}",
                            Reason = ex.Message,
                            RelatedOrderDetails = relatedOrderDetails
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa sản phẩm: {ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// Xóa dấu tiếng Việt (helper method)
        /// </summary>
        public string RemoveVietnameseAccents(string text)
        {
            string[] vietnameseChars = { "à", "á", "ạ", "ả", "ã", "â", "ầ", "ấ", "ậ", "ẩ", "ẫ", "ă", "ằ", "ắ", "ặ", "ẳ", "ẵ",
                                         "è", "é", "ẹ", "ẻ", "ẽ", "ê", "ề", "ế", "ệ", "ể", "ễ",
                                         "ì", "í", "ị", "ỉ", "ĩ",
                                         "ò", "ó", "ọ", "ỏ", "õ", "ô", "ồ", "ố", "ộ", "ổ", "ỗ", "ơ", "ờ", "ớ", "ợ", "ở", "ỡ",
                                         "ù", "ú", "ụ", "ủ", "ũ", "ư", "ừ", "ứ", "ự", "ử", "ữ",
                                         "ỳ", "ý", "ỵ", "ỷ", "ỹ",
                                         "đ" };

            string[] replacementChars = { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                                          "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e",
                                          "i", "i", "i", "i", "i",
                                          "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o",
                                          "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u",
                                          "y", "y", "y", "y", "y",
                                          "d" };

            string result = text;
            for (int i = 0; i < vietnameseChars.Length; i++)
            {
                result = result.Replace(vietnameseChars[i], replacementChars[i]);
                result = result.Replace(vietnameseChars[i].ToUpper(), replacementChars[i].ToUpper());
            }
            return result;
        }
        /// <summary>
        /// Import products from DataTable (Excel)
        /// Columns: ProductCode, ProductName, CategoryId, Description, SKU, Size, Color, ImportPrice, SellingPrice, Stock
        /// </summary>
        public async Task<(int success, int failed, List<string> errors)> ImportProductsFromExcelAsync(DataTable dt)
        {
            int success = 0;
            int failed = 0;
            List<string> errors = new List<string>();

            // Group by ProductCode
            var grouped = dt.AsEnumerable()
                .Where(row => !string.IsNullOrWhiteSpace(row["ProductCode"]?.ToString()))
                .GroupBy(row => row["ProductCode"].ToString());

            foreach (var group in grouped)
            {
                string productCode = group.Key;
                try
                {
                    // Basic product info from first row
                    DataRow firstRow = group.First();
                    string productName = firstRow["ProductName"]?.ToString() ?? "Unnamed Product";
                    int categoryId = 1; // Default
                    if (firstRow.Table.Columns.Contains("CategoryId") && int.TryParse(firstRow["CategoryId"]?.ToString(), out int cId))
                    {
                        categoryId = cId;
                    }
                    string desc = firstRow.Table.Columns.Contains("Description") ? firstRow["Description"]?.ToString() : "";

                    // Check if product exists by Code
                    // Need a method in DAL to check/get ID by Code.
                    // For now, try to Add, if duplicate code -> Exception or specific check?
                    // Currently AddProductAsync logic in DAL probably inserts.
                    // Let's assume we create new if not exists, or get ID if exists.
                    // But standard AddProductAsync fails if code exists (Unique constraint usually).
                    
                    // We need CheckProductExistsAsync(code)
                    int productId = await productDAL.GetProductIdByCodeAsync(productCode);
                    
                    if (productId == 0)
                    {
                        // Create new product
                        productId = await AddProductAsync(productCode, productName, categoryId, desc, null, true);
                    }

                    // Add variants
                    foreach (var row in group)
                    {
                        string sku = row["SKU"]?.ToString();
                        if (string.IsNullOrWhiteSpace(sku)) sku = productCode + "-" + Guid.NewGuid().ToString().Substring(0, 4);

                        string size = row.Table.Columns.Contains("Size") ? row["Size"]?.ToString() : null;
                        string color = row.Table.Columns.Contains("Color") ? row["Color"]?.ToString() : null;
                        
                        decimal importPrice = 0;
                        if (row.Table.Columns.Contains("ImportPrice")) decimal.TryParse(row["ImportPrice"]?.ToString(), out importPrice);
                        
                        decimal sellingPrice = 0;
                        if (row.Table.Columns.Contains("SellingPrice")) decimal.TryParse(row["SellingPrice"]?.ToString(), out sellingPrice);

                        int stock = 0;
                        if (row.Table.Columns.Contains("Stock")) int.TryParse(row["Stock"]?.ToString(), out stock);

                        // Check if variant SKU exists?
                        // AddProductVariantAsync usually adds.
                        await AddProductVariantAsync(productId, sku, size, color, importPrice, sellingPrice, stock, null, true);
                    }
                    success++;
                }
                catch (Exception ex)
                {
                    failed++;
                    errors.Add($"Product {productCode}: {ex.Message}");
                }
            }

            return (success, failed, errors);
        }
    }
}

