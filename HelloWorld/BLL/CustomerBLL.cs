using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace helloworld
{
    /// <summary>
    /// Business Logic Layer cho Customers
    /// Chứa business logic, validation, và gọi CustomerDAL
    /// Giữ nguyên interface public như KhachHangModels cũ để tương thích với Forms
    /// </summary>
    public class CustomerBLL
    {
        private CustomerDAL customerDAL;

        public CustomerBLL()
        {
            customerDAL = new CustomerDAL();
        }

        /// <summary>
        /// Thêm khách hàng mới vào database
        /// </summary>
        public async Task<int> AddCustomerAsync(Customer customer)
        {
            // Validate dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(customer.FullName))
            {
                throw new ArgumentException("Họ và tên không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(customer.PhoneNumber))
            {
                throw new ArgumentException("Số điện thoại không được để trống.");
            }

            return await customerDAL.AddCustomerAsync(customer);
        }

        /// <summary>
        /// Kiểm tra khách hàng có liên quan đến hóa đơn nào không
        /// </summary>
        public async Task<List<OrderInfo>> CheckCustomerOrdersAsync(int customerId)
        {
            return await customerDAL.CheckCustomerOrdersAsync(customerId);
        }

        /// <summary>
        /// Xóa khách hàng khỏi database (có kiểm tra hóa đơn)
        /// </summary>
        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            // Kiểm tra xem khách hàng có liên quan đến hóa đơn không
            var orders = await CheckCustomerOrdersAsync(customerId);
            if (orders.Count > 0)
            {
                throw new Exception($"Không thể xóa khách hàng vì đã có {orders.Count} hóa đơn liên quan.");
            }

            return await customerDAL.DeleteCustomerAsync(customerId);
        }

        /// <summary>
        /// Xóa nhiều khách hàng cùng lúc
        /// </summary>
        public async Task<DeleteResult> DeleteCustomersAsync(List<int> customerIds)
        {
            DeleteResult result = new DeleteResult();

            foreach (int customerId in customerIds)
            {
                try
                {
                    // Kiểm tra xem khách hàng có liên quan đến hóa đơn không
                    var orders = await CheckCustomerOrdersAsync(customerId);
                    if (orders.Count > 0)
                    {
                        result.FailedCustomers.Add(new FailedDeleteInfo
                        {
                            CustomerId = customerId,
                            Reason = $"Có {orders.Count} hóa đơn liên quan",
                            RelatedOrders = orders
                        });
                        continue;
                    }

                    // Thực hiện xóa
                    await customerDAL.DeleteCustomerAsync(customerId);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailedCustomers.Add(new FailedDeleteInfo
                    {
                        CustomerId = customerId,
                        Reason = ex.Message,
                        RelatedOrders = new List<OrderInfo>()
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Lấy thông tin khách hàng theo ID (bao gồm image BLOB)
        /// </summary>
        public async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            return await customerDAL.GetCustomerByIdAsync(customerId);
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng (bao gồm image)
        /// </summary>
        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            // Validate dữ liệu đầu vào
            if (customer.CustomerId <= 0)
            {
                throw new ArgumentException("Customer ID không hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(customer.FullName))
            {
                throw new ArgumentException("Họ và tên không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(customer.PhoneNumber))
            {
                throw new ArgumentException("Số điện thoại không được để trống.");
            }

            return await customerDAL.UpdateCustomerAsync(customer);
        }

        /// <summary>
        /// Lấy danh sách tất cả khách hàng
        /// </summary>
        public async Task<DataTable> LoadCustomersAsync()
        {
            return await customerDAL.LoadCustomersAsync();
        }

        /// <summary>
        /// Tìm kiếm khách hàng theo mã, số điện thoại, email, tên, địa chỉ
        /// </summary>
        public async Task<DataTable> SearchCustomersAsync(string keyword)
        {
            return await customerDAL.SearchCustomersAsync(keyword);
        }

        /// <summary>
        /// Đếm tổng số lượng khách hàng
        /// </summary>
        public async Task<int> GetTotalCustomerCountAsync()
        {
            return await customerDAL.GetTotalCustomerCountAsync();
        }

        /// <summary>
        /// Lấy danh sách hóa đơn của khách hàng dưới dạng DataTable (để hiển thị trong DataGridView)
        /// </summary>
        public async Task<DataTable> GetCustomerOrdersDataTableAsync(int customerId)
        {
            List<OrderInfo> orders = await CheckCustomerOrdersAsync(customerId);
            
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Mã HĐ", typeof(int));
            dataTable.Columns.Add("Mã hóa đơn", typeof(string));
            dataTable.Columns.Add("Tổng tiền", typeof(decimal));
            dataTable.Columns.Add("Phương thức thanh toán", typeof(string));
            dataTable.Columns.Add("Trạng thái", typeof(string));
            dataTable.Columns.Add("Ngày tạo", typeof(DateTime));
            dataTable.Columns.Add("Nhân viên", typeof(string));

            foreach (var order in orders)
            {
                dataTable.Rows.Add(
                    order.OrderId,
                    order.OrderCode,
                    order.TotalAmount,
                    order.PaymentMethod,
                    order.Status,
                    order.CreatedAt,
                    order.UserName
                );
            }

            return dataTable;
        }
    }
}

