using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using helloworld.DAL;
using helloworld.Models;
using helloworld.Services;

namespace helloworld.BLL
{
    public class WooCommerceBLL
    {
        private readonly WooCommerceService _service;
        private readonly ProductDAL _productDAL;
        private readonly CategoryDAL _categoryDAL; // Giả sử đã có CategoryDAL, cần check lại project structure

        public WooCommerceBLL()
        {
            _service = new WooCommerceService();
            _productDAL = new ProductDAL();
            _categoryDAL = new CategoryDAL(); 
        }

        public async Task<string> SyncProductsAsync(string domain, string key, string secret, Action<string> progressReport)
        {
            int page = 1;
            int syncCount = 0;
            int errorCount = 0;

            while (true)
            {
                progressReport?.Invoke($"Đang tải trang {page}...");
                try
                {
                    var wcProducts = await _service.GetProductsAsync(domain, key, secret, page, 10);
                    if (wcProducts.Count == 0) break;

                    foreach (var wcP in wcProducts)
                    {
                        try
                        {
                            await ProcessProductAsync(wcP, domain, key, secret);
                            syncCount++;
                            progressReport?.Invoke($"Đã đồng bộ sản phẩm: {wcP.name}");
                        }
                        catch (Exception innerEx)
                        {
                            errorCount++;
                            progressReport?.Invoke($"Lỗi đồng bộ SP {wcP.name}: {innerEx.Message}");
                        }
                    }

                    page++;
                }
                catch (Exception ex)
                {
                    return $"Lỗi khi tải trang {page}: {ex.Message}";
                }
            }

            return $"Hoàn tất! Thành công: {syncCount}, Lỗi: {errorCount}";
        }

        private async Task ProcessProductAsync(WcProduct wcP, string domain, string key, string secret)
        {
            // 1. Xử lý Category (lấy cái đầu tiên nếu có)
            int? categoryId = null;
            if (wcP.categories != null && wcP.categories.Count > 0)
            {
                // Logic đơn giản: mapping theo tên, nếu chưa có thì có thể tạo hoặc bỏ qua.
                // Ở đây ta tạm bỏ qua logic tạo category phức tạp, chỉ set null nếu không map được
                // Hoặc TODO: Implement GetCategoryIdByNameAsync in CategoryDAL
            }

            // 2. Map Product
            // Kiểm tra xem sản phẩm đã tồn tại chưa (dựa theo ProductCode = SKU hoặc Name)
            // Ưu tiên dùng SKU từ WooCommerce làm ProductCode
            string productCode = !string.IsNullOrEmpty(wcP.sku) ? wcP.sku : $"WC_{wcP.id}";
            
            // Check existence
            var existingId = await _productDAL.GetProductIdByCodeAsync(productCode);
            
            var product = new Product
            {
                ProductCode = productCode,
                ProductName = wcP.name,
                Description = CleanHtml(wcP.description ?? wcP.short_description),
                BaseImagePath = wcP.images != null && wcP.images.Count > 0 ? wcP.images[0].src : null,
                IsActive = wcP.status == "publish",
                CategoryId = categoryId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            int productId;
            if (existingId > 0)
            {
                // Update
                product.ProductId = existingId;
                await _productDAL.UpdateProductAsync(existingId, product.ProductName, product.CategoryId, product.Description, product.BaseImagePath, product.IsActive);
                productId = existingId;
            }
            else
            {
                // Insert
                productId = await _productDAL.AddProductAsync(product.ProductCode, product.ProductName, product.CategoryId, product.Description, product.BaseImagePath, product.IsActive);
            }

            // 3. Xử lý Variations (ProductVariant)
            // Nếu là variable product thì fetch variations, nếu simple thì dùng price chính nó làm variant default
            if (wcP.variations != null && wcP.variations.Count > 0)
            {
                var variations = await _service.GetProductVariationsAsync(domain, key, secret, wcP.id);
                foreach (var v in variations)
                {
                    await ProcessVariantAsync(productId, v);
                }
            }
            else
            {
                // Simple product -> tạo 1 variant mặc định
                if (decimal.TryParse(wcP.price, out decimal price))
                {
                     var variant = new ProductVariant
                     {
                         ProductId = productId,
                         SKU = !string.IsNullOrEmpty(wcP.sku) ? wcP.sku : productCode,
                         ImportPrice = 0, // WC API không trả giá nhập -> set 0
                         SellingPrice = price,
                         StockQuantity = wcP.stock_quantity ?? 0,
                         IsActive = true,
                         ImagePath = wcP.images != null && wcP.images.Count > 0 ? wcP.images[0].src : null
                     };
                     
                     // Check if variant exists? 
                     // Hiện tại ProductDAL.AddProductVariantAsync thường insert mới. 
                     // Để tránh duplicate, ta nên check. Tuy nhiên để đơn giản bước đầu ta delete all variants cũ hoặc check SKU.
                     // TODO: Improve logic check existent variant
                     await _productDAL.AddProductVariantAsync(variant.ProductId, variant.SKU, variant.Size, variant.Color, variant.ImportPrice, variant.SellingPrice, variant.StockQuantity, variant.ImagePath, variant.IsActive);
                }
            }
        }

        private async Task ProcessVariantAsync(int productId, WcVariation v)
        {
             if (decimal.TryParse(v.price, out decimal price))
             {
                 var variant = new ProductVariant
                 {
                     ProductId = productId,
                     SKU = !string.IsNullOrEmpty(v.sku) ? v.sku : $"WC_VAR_{v.id}",
                     ImportPrice = 0,
                     SellingPrice = price,
                     StockQuantity = v.stock_quantity ?? 0,
                     IsActive = true,
                     ImagePath = v.image?.src
                 };
                 // Size/Color mapping từ attributes
                 foreach (var attr in v.attributes)
                 {
                     if (attr.name.ToLower().Contains("size") || attr.name.ToLower().Contains("kích thước"))
                        variant.Size = attr.option;
                     if (attr.name.ToLower().Contains("color") || attr.name.ToLower().Contains("màu"))
                        variant.Color = attr.option;
                 }

                 await _productDAL.AddProductVariantAsync(variant.ProductId, variant.SKU, variant.Size, variant.Color, variant.ImportPrice, variant.SellingPrice, variant.StockQuantity, variant.ImagePath, variant.IsActive);
             }
        }

        private string CleanHtml(string html)
        {
            // Đơn giản hóa, có thể dùng Regex để strip HTML tags
            if (string.IsNullOrEmpty(html)) return null;
            return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", String.Empty);
        }
    }
}
