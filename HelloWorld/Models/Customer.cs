using System;
using System.Collections.Generic;

namespace helloworld
{
    /// <summary>
    /// Entity class đại diện cho khách hàng
    /// </summary>
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public byte[]? Image { get; set; } // Ảnh khách hàng dạng BLOB
        public int Points { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

