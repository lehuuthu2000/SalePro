using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace helloworld
{
    /// <summary>
    /// Business Logic Layer cho Orders và OrderDetails
    /// Chứa business logic, validation, và gọi OrderDAL
    /// Giữ nguyên interface public như HoaDonModels cũ để tương thích với Forms
    /// </summary>
    internal class OrderBLL
    {
        private OrderDAL orderDAL;

        public OrderBLL()
        {
            orderDAL = new OrderDAL();
        }

        /// <summary>
        /// Lấy danh sách hóa đơn từ bảng Orders
        /// </summary>
        public async Task<DataTable> LoadOrdersAsync()
        {
            return await orderDAL.LoadOrdersAsync();
        }

        /// <summary>
        /// Tạo mã hóa đơn tự động (VD: HD20231010001)
        /// </summary>
        public async Task<string> GenerateOrderCodeAsync()
        {
            try
            {
                int orderCount = await orderDAL.GetTodayOrderCountAsync();
                int orderNumber = orderCount + 1;
                return $"HD{DateTime.Now:yyyyMMdd}{orderNumber:D3}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo mã hóa đơn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin khách hàng theo ID
        /// </summary>
        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            return await orderDAL.GetCustomerByIdAsync(customerId);
        }

        /// <summary>
        /// Lấy thông tin khách hàng theo số điện thoại
        /// </summary>
        public async Task<Customer?> GetCustomerByPhoneAsync(string phoneNumber)
        {
            return await orderDAL.GetCustomerByPhoneAsync(phoneNumber);
        }

        /// <summary>
        /// Lấy thông tin variant theo ID (không kiểm tra is_active - dùng cho load hóa đơn cũ)
        /// </summary>
        public async Task<ProductVariantInfo?> GetVariantInfoByIdAsync(int variantId)
        {
            return await orderDAL.GetVariantInfoByIdAsync(variantId);
        }

        /// <summary>
        /// Lấy danh sách ProductVariants có sẵn để bán
        /// </summary>
        public async Task<List<ProductVariantInfo>> GetProductVariantsAsync()
        {
            return await orderDAL.GetProductVariantsAsync();
        }

        /// <summary>
        /// Thêm hóa đơn mới vào database
        /// </summary>
        public async Task<int> AddOrderAsync(Order order)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(order.OrderCode))
            {
                throw new ArgumentException("Mã hóa đơn không được để trống.");
            }

            if (order.UserId <= 0)
            {
                throw new ArgumentException("Mã nhân viên không hợp lệ.");
            }

            return await orderDAL.AddOrderAsync(order);
        }

        /// <summary>
        /// Thêm chi tiết hóa đơn vào database
        /// </summary>
        public async Task AddOrderDetailAsync(OrderDetail orderDetail)
        {
            // Validate
            if (orderDetail.OrderId <= 0)
            {
                throw new ArgumentException("Order ID không hợp lệ.");
            }

            if (orderDetail.VariantId <= 0)
            {
                throw new ArgumentException("Variant ID không hợp lệ.");
            }

            if (orderDetail.Quantity <= 0)
            {
                throw new ArgumentException("Số lượng phải lớn hơn 0.");
            }

            await orderDAL.AddOrderDetailAsync(orderDetail);
        }

        /// <summary>
        /// Thêm nhiều chi tiết hóa đơn cùng lúc (transaction)
        /// </summary>
        public async Task AddOrderDetailsAsync(List<OrderDetail> orderDetails)
        {
            await orderDAL.AddOrderDetailsAsync(orderDetails);
        }

        /// <summary>
        /// Lấy thông tin hóa đơn theo Order ID
        /// </summary>
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await orderDAL.GetOrderByIdAsync(orderId);
        }

        /// <summary>
        /// Lấy thông tin hóa đơn kèm ngày tạo và ngày cập nhật
        /// </summary>
        public async Task<OrderWithDates?> GetOrderWithDatesAsync(int orderId)
        {
            return await orderDAL.GetOrderWithDatesAsync(orderId);
        }

        /// <summary>
        /// Lấy danh sách chi tiết hóa đơn theo Order ID
        /// </summary>
        public async Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            return await orderDAL.GetOrderDetailsByOrderIdAsync(orderId);
        }

        /// <summary>
        /// Cập nhật số lượng tồn kho của variant (giảm khi bán)
        /// </summary>
        public async Task UpdateVariantStockAsync(int variantId, int quantity)
        {
            await orderDAL.UpdateVariantStockAsync(variantId, quantity);
        }

        /// <summary>
        /// Cập nhật số tiền bán và số lượng bán của variant
        /// </summary>
        public async Task UpdateVariantSalesAsync(int variantId, int quantity, decimal amount)
        {
            await orderDAL.UpdateVariantSalesAsync(variantId, quantity, amount);
        }

        /// <summary>
        /// Xử lý khi hóa đơn chuyển sang trạng thái "Completed" - Cập nhật tồn kho, số lượng bán và số tiền bán
        /// </summary>
        public async Task ProcessOrderCompletionAsync(int orderId)
        {
            try
            {
                // Lấy danh sách OrderDetails
                List<OrderDetail> orderDetails = await GetOrderDetailsByOrderIdAsync(orderId);

                if (orderDetails == null || orderDetails.Count == 0)
                {
                    return;
                }

                // Cập nhật tồn kho, số lượng bán và số tiền bán cho từng variant
                foreach (var detail in orderDetails)
                {
                    // Giảm tồn kho
                    await UpdateVariantStockAsync(detail.VariantId, -detail.Quantity);
                    
                    // Tăng số lượng bán và số tiền bán
                    await UpdateVariantSalesAsync(detail.VariantId, detail.Quantity, detail.Subtotal);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xử lý hoàn tất hóa đơn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Hoàn lại số lượng bán và số tiền bán khi hóa đơn chuyển từ "Completed" sang trạng thái khác (Pending/Cancelled)
        /// Lưu ý: KHÔNG hoàn lại tồn kho vì hàng đã xuất, chỉ là chưa thanh toán hoặc đã hủy
        /// </summary>
        public async Task RevertOrderCompletionAsync(int orderId)
        {
            try
            {
                // Lấy danh sách OrderDetails
                List<OrderDetail> orderDetails = await GetOrderDetailsByOrderIdAsync(orderId);

                if (orderDetails == null || orderDetails.Count == 0)
                {
                    return;
                }

                // Chỉ trừ số lượng bán và số tiền bán, KHÔNG hoàn lại tồn kho
                foreach (var detail in orderDetails)
                {
                    // Giảm số lượng bán và số tiền bán (trừ đi)
                    await UpdateVariantSalesAsync(detail.VariantId, -detail.Quantity, -detail.Subtotal);
                    // KHÔNG tăng lại tồn kho vì hàng đã xuất
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi hoàn lại số liệu bán hàng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin hóa đơn
        /// </summary>
        public async Task UpdateOrderAsync(Order order)
        {
            if (order.OrderId <= 0)
            {
                throw new ArgumentException("Order ID không hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(order.OrderCode))
            {
                throw new ArgumentException("Mã hóa đơn không được để trống.");
            }

            string? oldStatus = await orderDAL.GetOldOrderStatusAsync(order.OrderId);

            await orderDAL.UpdateOrderAsync(order);

            // Xử lý business logic khi thay đổi trạng thái
            if (oldStatus != null && oldStatus != order.Status)
            {
                if (oldStatus == "Completed" && order.Status != "Completed")
                {
                    // Hoàn lại tồn kho nếu chuyển từ Completed sang trạng thái khác
                    await RevertOrderCompletionAsync(order.OrderId);
                }
                else if (oldStatus != "Completed" && order.Status == "Completed")
                {
                    // Cập nhật tồn kho nếu chuyển sang Completed
                    await ProcessOrderCompletionAsync(order.OrderId);
                }
            }
        }

        /// <summary>
        /// Xóa tất cả chi tiết hóa đơn theo Order ID
        /// </summary>
        public async Task DeleteOrderDetailsAsync(int orderId)
        {
            await orderDAL.DeleteOrderDetailsAsync(orderId);
        }

        /// <summary>
        /// Xóa hóa đơn (xóa cả OrderDetails và Order)
        /// QUAN TRỌNG: Nếu hóa đơn có status = "Completed", cần hoàn lại tồn kho và số liệu bán hàng trước khi xóa
        /// </summary>
        public async Task DeleteOrderAsync(int orderId)
        {
            if (orderId <= 0)
            {
                throw new ArgumentException("Order ID không hợp lệ.");
            }

            // Kiểm tra trạng thái hóa đơn trước khi xóa
            Order? order = await GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy hóa đơn để xóa.");
            }

            // Nếu hóa đơn đã Completed, cần hoàn lại số liệu bán hàng
            // QUAN TRỌNG: Phải lấy OrderDetails TRƯỚC khi xóa, vì sau khi xóa sẽ không lấy được
            // Lưu ý: KHÔNG hoàn lại tồn kho vì hàng đã xuất, chỉ trừ số lượng bán và số tiền bán
            if (order.Status == "Completed")
            {
                // Lấy danh sách OrderDetails trước khi xóa
                List<OrderDetail> orderDetails = await GetOrderDetailsByOrderIdAsync(orderId);

                if (orderDetails != null && orderDetails.Count > 0)
                {
                    // Chỉ trừ số lượng bán và số tiền bán, KHÔNG hoàn lại tồn kho
                    foreach (var detail in orderDetails)
                    {
                        // Giảm số lượng bán và số tiền bán (vì đã tăng khi Completed)
                        await UpdateVariantSalesAsync(detail.VariantId, -detail.Quantity, -detail.Subtotal);
                        // KHÔNG tăng lại tồn kho vì hàng đã xuất
                    }
                }
            }

            // Sau đó mới xóa hóa đơn (xóa OrderDetails và Order)
            await orderDAL.DeleteOrderAsync(orderId);
        }

        /// <summary>
        /// Xóa nhiều hóa đơn cùng lúc
        /// </summary>
        public async Task<DeleteOrderResult> DeleteOrdersAsync(List<int> orderIds)
        {
            DeleteOrderResult result = new DeleteOrderResult();

            if (orderIds == null || orderIds.Count == 0)
                return result;

            try
            {
                foreach (int orderId in orderIds)
                {
                    try
                    {
                        // Lấy thông tin hóa đơn để hiển thị nếu lỗi
                        string orderCode = $"Order ID: {orderId}";
                        
                        try
                        {
                            Order? order = await GetOrderByIdAsync(orderId);
                            if (order != null)
                            {
                                orderCode = order.OrderCode;
                            }
                        }
                        catch
                        {
                            // Nếu không lấy được thông tin, dùng orderId
                        }

                        // Thực hiện xóa (gọi DeleteOrderAsync của BLL để có logic hoàn lại tồn kho)
                        await DeleteOrderAsync(orderId);
                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        // Lấy lại orderCode nếu cần (trong trường hợp lỗi)
                        string orderCode = $"Order ID: {orderId}";
                        try
                        {
                            Order? order = await GetOrderByIdAsync(orderId);
                            if (order != null)
                            {
                                orderCode = order.OrderCode;
                            }
                        }
                        catch
                        {
                            // Nếu không lấy được, dùng orderId
                        }

                        result.FailedOrders.Add(new FailedDeleteOrderInfo
                        {
                            OrderId = orderId,
                            OrderCode = orderCode,
                            Reason = ex.Message
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa hóa đơn: {ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// Tính tổng doanh số từ các hóa đơn đã thanh toán (status = 'Completed')
        /// </summary>
        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await orderDAL.GetTotalRevenueAsync();
        }

        /// <summary>
        /// Lấy danh sách hóa đơn gần nhất
        /// </summary>
        public async Task<List<OrderInfo>> GetRecentOrdersAsync(int limit = 5)
        {
            return await orderDAL.GetRecentOrdersAsync(limit);
        }

        /// <summary>
        /// Tính lại số tiền bán và số lượng bán của variant dựa trên tất cả hóa đơn Completed
        /// Đồng thời tính lại tồn kho
        /// </summary>
        public async Task RecalculateVariantSalesAsync(int variantId)
        {
            await orderDAL.RecalculateVariantSalesAsync(variantId);
        }

        /// <summary>
        /// Tính lại số tiền bán và số lượng bán của tất cả variants trong một sản phẩm
        /// Đồng thời tính lại tồn kho
        /// </summary>
        public async Task RecalculateProductSalesAsync(int productId)
        {
            await orderDAL.RecalculateProductSalesAsync(productId);
        }

        /// <summary>
        /// Tính lại tồn kho của variant dựa trên số lượng đã bán trong hóa đơn Completed
        /// </summary>
        public async Task RecalculateVariantStockAsync(int variantId)
        {
            await orderDAL.RecalculateVariantStockAsync(variantId);
        }
    }
}

