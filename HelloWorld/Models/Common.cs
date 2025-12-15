using System;
using System.Collections.Generic;

namespace helloworld
{
    /// <summary>
    /// Class chứa thông tin hóa đơn
    /// </summary>
    public class OrderInfo
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
    }

    /// <summary>
    /// Class chứa kết quả xóa nhiều khách hàng
    /// </summary>
    public class DeleteResult
    {
        public int SuccessCount { get; set; } = 0;
        public List<FailedDeleteInfo> FailedCustomers { get; set; } = new List<FailedDeleteInfo>();
    }

    /// <summary>
    /// Class chứa thông tin khách hàng không thể xóa
    /// </summary>
    public class FailedDeleteInfo
    {
        public int CustomerId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public List<OrderInfo> RelatedOrders { get; set; } = new List<OrderInfo>();
    }

    /// <summary>
    /// Class chứa kết quả xóa nhiều hóa đơn
    /// </summary>
    public class DeleteOrderResult
    {
        public int SuccessCount { get; set; } = 0;
        public List<FailedDeleteOrderInfo> FailedOrders { get; set; } = new List<FailedDeleteOrderInfo>();
    }

    /// <summary>
    /// Class chứa thông tin hóa đơn không thể xóa
    /// </summary>
    public class FailedDeleteOrderInfo
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}

