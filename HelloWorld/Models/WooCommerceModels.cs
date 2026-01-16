using System;
using System.Collections.Generic;

namespace helloworld.Models
{
    // Các class này map với JSON response của WooCommerce API v3

    public class WcProduct
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string sku { get; set; } = string.Empty; // Quan trọng để map với ProductCode
        public string price { get; set; }
        public string regular_price { get; set; }
        public string sale_price { get; set; }
        public string description { get; set; }
        public string short_description { get; set; }
        public bool manage_stock { get; set; }
        public int? stock_quantity { get; set; }
        public string status { get; set; } // publish, draft, etc.
        public List<WcCategory> categories { get; set; } = new List<WcCategory>();
        public List<WcImage> images { get; set; } = new List<WcImage>();
        public List<int> variations { get; set; } = new List<int>(); // List ID của variations
        public DateTime date_created { get; set; }
        public DateTime date_modified { get; set; }
    }

    public class WcCategory
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string slug { get; set; } = string.Empty;
    }

    public class WcImage
    {
        public int id { get; set; }
        public string src { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string alt { get; set; } = string.Empty;
    }

    public class WcVariation
    {
        public int id { get; set; }
        public string sku { get; set; } = string.Empty;
        public string price { get; set; }
        public string regular_price { get; set; }
        public string sale_price { get; set; }
        public bool manage_stock { get; set; }
        public int? stock_quantity { get; set; }
        public WcImage image { get; set; }
        public List<WcAttribute> attributes { get; set; } = new List<WcAttribute>();
    }

    public class WcAttribute
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string option { get; set; } = string.Empty;
    }
}
