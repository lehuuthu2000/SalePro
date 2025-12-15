using System;

namespace helloworld
{
    /// <summary>
    /// Entity class đại diện cho sản phẩm
    /// </summary>
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string? Description { get; set; }
        public string? BaseImagePath { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

