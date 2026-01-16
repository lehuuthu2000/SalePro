using System;
using System.Collections.Generic;

namespace helloworld
{
    /// <summary>
    /// Entity class đại diện cho hóa đơn
    /// </summary>
    public class Order
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public int UserId { get; set; }
        public string CreatorName { get; set; } = string.Empty;
        public decimal Tax { get; set; } = 0;
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public string PaymentMethod { get; set; } = "Cash";
        public string Status { get; set; } = "Completed";
        public string? ShippingAddress { get; set; }
        public string? BillingAddress { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Entity class đại diện cho chi tiết hóa đơn
    /// </summary>
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int VariantId { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

    /// <summary>
    /// Entity class đại diện cho ProductVariant (để hiển thị trong form chọn sản phẩm)
    /// </summary>
    public class ProductVariantInfo
    {
        public int VariantId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal SellingPrice { get; set; }
        public int StockQuantity { get; set; }
    }

    /// <summary>
    /// Class chứa thông tin ngày tạo và ngày cập nhật của hóa đơn
    /// </summary>
    public class OrderWithDates
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

