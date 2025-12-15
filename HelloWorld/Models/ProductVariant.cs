namespace helloworld
{
    /// <summary>
    /// Entity class đại diện cho biến thể sản phẩm
    /// </summary>
    public class ProductVariant
    {
        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal ImportPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int StockQuantity { get; set; }
        public int TotalSalesQuantity { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public string? ImagePath { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Class chứa tổng hợp thông tin sản phẩm
    /// </summary>
    public class ProductSummary
    {
        public int TotalStockQuantity { get; set; }
        public int TotalSalesQuantity { get; set; }
        public decimal TotalSalesAmount { get; set; }
    }

    /// <summary>
    /// Class chứa kết quả xóa biến thể
    /// </summary>
    public class DeleteVariantResult
    {
        public int SuccessCount { get; set; } = 0;
        public List<FailedDeleteVariantInfo> FailedVariants { get; set; } = new List<FailedDeleteVariantInfo>();
    }

    /// <summary>
    /// Class chứa thông tin biến thể không thể xóa
    /// </summary>
    public class FailedDeleteVariantInfo
    {
        public int VariantId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }

    /// <summary>
    /// Class chứa kết quả xóa sản phẩm
    /// </summary>
    public class DeleteProductResult
    {
        public int SuccessCount { get; set; } = 0;
        public List<FailedDeleteProductInfo> FailedProducts { get; set; } = new List<FailedDeleteProductInfo>();
    }

    /// <summary>
    /// Class chứa thông tin sản phẩm không thể xóa
    /// </summary>
    public class FailedDeleteProductInfo
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public List<RelatedOrderDetail> RelatedOrderDetails { get; set; } = new List<RelatedOrderDetail>();
    }

    /// <summary>
    /// Class chứa thông tin chi tiết hóa đơn liên quan
    /// </summary>
    public class RelatedOrderDetail
    {
        public string OrderCode { get; set; } = string.Empty;
        public string VariantSKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}

