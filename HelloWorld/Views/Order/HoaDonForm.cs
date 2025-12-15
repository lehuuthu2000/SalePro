using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace helloworld
{
    public partial class HoaDonForm : Form
    {
        private OrderBLL hoaDonModels; // Đã chuyển sang OrderBLL
        private DataTable orderDetailsTable;
        private BindingSource orderDetailsBindingSource = new BindingSource(); // BindingSource cho OrderDetails
        private int? customerId = null;
        private int? orderId = null; // Order ID khi ở chế độ edit
        private bool isEditMode = false; // Flag để phân biệt Add/Edit mode

        /// <summary>
        /// Constructor cho chế độ thêm mới
        /// </summary>
        public HoaDonForm()
        {
            InitializeComponent();
            hoaDonModels = new OrderBLL(); // Đã chuyển sang OrderBLL
            InitializeOrderDetailsTable();
            this.Load += HoaDonForm_Load;
            buttonThemSanPham.Click += ButtonThemSanPham_Click;
            buttonXoa.Click += ButtonXoa_Click;
            buttonLuu.Click += ButtonLuu_Click;
            textBoxSoDienThoai.TextChanged += TextBoxSoDienThoai_TextChanged;
            textBoxGiamGia.TextChanged += TextBoxGiamGia_TextChanged;
            textBoxTax.TextChanged += TextBoxTax_TextChanged;
        }

        /// <summary>
        /// Constructor cho chế độ chỉnh sửa
        /// </summary>
        /// <param name="orderId">Order ID cần chỉnh sửa</param>
        public HoaDonForm(int orderId) : this()
        {
            this.orderId = orderId;
            this.isEditMode = true;
        }

        /// <summary>
        /// Khởi tạo DataTable cho OrderDetails
        /// </summary>
        private void InitializeOrderDetailsTable()
        {
            orderDetailsTable = new DataTable();
            orderDetailsTable.Columns.Add("VariantId", typeof(int));
            orderDetailsTable.Columns.Add("SKU", typeof(string));
            orderDetailsTable.Columns.Add("Tên sản phẩm", typeof(string));
            orderDetailsTable.Columns.Add("Size", typeof(string));
            orderDetailsTable.Columns.Add("Màu", typeof(string));
            orderDetailsTable.Columns.Add("Số lượng", typeof(int));
            orderDetailsTable.Columns.Add("Đơn giá", typeof(decimal));
            orderDetailsTable.Columns.Add("Thành tiền", typeof(decimal));

            // Bind DataTable vào BindingSource
            orderDetailsBindingSource.DataSource = orderDetailsTable;
            
            // Gắn BindingSource vào DataGridView
            dataGridViewSanPham.DataSource = orderDetailsBindingSource;
            
            ConfigureDataGridViewSanPham();
        }

        /// <summary>
        /// Cấu hình DataGridView cho sản phẩm
        /// </summary>
        private void ConfigureDataGridViewSanPham()
        {
            dataGridViewSanPham.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewSanPham.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewSanPham.MultiSelect = true;
            dataGridViewSanPham.ReadOnly = false; // Cho phép chỉnh sửa số lượng

            // Ẩn cột VariantId (chỉ dùng để lưu)
            if (dataGridViewSanPham.Columns["VariantId"] != null)
            {
                dataGridViewSanPham.Columns["VariantId"].Visible = false;
            }

            // Format cột tiền tệ
            if (dataGridViewSanPham.Columns["Đơn giá"] != null)
            {
                dataGridViewSanPham.Columns["Đơn giá"].DefaultCellStyle.Format = "N0";
                dataGridViewSanPham.Columns["Đơn giá"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dataGridViewSanPham.Columns["Thành tiền"] != null)
            {
                dataGridViewSanPham.Columns["Thành tiền"].DefaultCellStyle.Format = "N0";
                dataGridViewSanPham.Columns["Thành tiền"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            // Format cột số lượng
            if (dataGridViewSanPham.Columns["Số lượng"] != null)
            {
                dataGridViewSanPham.Columns["Số lượng"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // Event để tính lại thành tiền khi số lượng thay đổi
            dataGridViewSanPham.CellValueChanged += DataGridViewSanPham_CellValueChanged;
        }

        /// <summary>
        /// Tính lại thành tiền khi số lượng thay đổi
        /// </summary>
        private void DataGridViewSanPham_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridViewSanPham.Columns["Số lượng"].Index)
            {
                DataGridViewRow row = dataGridViewSanPham.Rows[e.RowIndex];
                if (row.Cells["Số lượng"].Value != null && row.Cells["Đơn giá"].Value != null)
                {
                    int quantity = Convert.ToInt32(row.Cells["Số lượng"].Value);
                    decimal unitPrice = Convert.ToDecimal(row.Cells["Đơn giá"].Value);
                    row.Cells["Thành tiền"].Value = quantity * unitPrice;
                    CalculateTotal();
                }
            }
        }

        /// <summary>
        /// Load form - Tạo mã hóa đơn và hiển thị ngày (hoặc load dữ liệu nếu edit mode)
        /// </summary>
        private async void HoaDonForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Hiển thị thông tin nhân viên đang đăng nhập
                var currentUser = UserSession.GetCurrentUser();
                if (currentUser != null)
                {
                    textBoxNhanVien.Text = currentUser.FullName;
                    textBoxNhanVien.ReadOnly = true; // Chỉ đọc, không cho sửa
                }
                else
                {
                    textBoxNhanVien.Text = "Chưa đăng nhập";
                    textBoxNhanVien.ReadOnly = true;
                }

                if (isEditMode && orderId.HasValue)
                {
                    // Chế độ chỉnh sửa - Load dữ liệu hóa đơn
                    await LoadOrderDataAsync(orderId.Value);
                }
                else
                {
                    // Chế độ thêm mới - Tạo mã hóa đơn tự động
                    string orderCode = await hoaDonModels.GenerateOrderCodeAsync();
                    labelMaHoaDon.Text = orderCode;

                    // Hiển thị ngày tạo
                    labelNgayTao.Text = $"Ngày tạo: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    label11.Text = $"Ngày cập nhật: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

                    // Set mặc định status
                    comboBox1.SelectedIndex = 2; // "Đã thanh toán"
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi tạo form: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load dữ liệu hóa đơn vào form (chế độ edit)
        /// </summary>
        private async Task LoadOrderDataAsync(int orderId)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Lấy thông tin hóa đơn
                Order? order = await hoaDonModels.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Hiển thị thông tin hóa đơn
                labelMaHoaDon.Text = order.OrderCode;
                textBoxGiamGia.Text = order.DiscountAmount.ToString("N0");
                textBoxTax.Text = order.Tax.ToString("N0");
                textBoxTongTien.Text = order.TotalAmount.ToString("N0");
                textBox1.Text = order.BillingAddress ?? "";
                textBox2.Text = order.ShippingAddress ?? "";

                // Lấy ngày tạo và ngày cập nhật từ database
                try
                {
                    var orderWithDates = await hoaDonModels.GetOrderWithDatesAsync(orderId);
                    if (orderWithDates != null)
                    {
                        labelNgayTao.Text = $"Ngày tạo: {orderWithDates.CreatedAt:dd/MM/yyyy HH:mm:ss}";
                        label11.Text = $"Ngày cập nhật: {orderWithDates.UpdatedAt:dd/MM/yyyy HH:mm:ss}";
                    }
                }
                catch
                {
                    // Nếu có lỗi, dùng giá trị mặc định
                    labelNgayTao.Text = $"Ngày tạo: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    label11.Text = $"Ngày cập nhật: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                }

                // Set status
                if (order.Status == "Completed")
                    comboBox1.SelectedIndex = 2; // "Đã thanh toán"
                else if (order.Status == "Pending")
                    comboBox1.SelectedIndex = 1; // "Chưa thanh toán"
                else
                    comboBox1.SelectedIndex = 0; // "Nháp"

                // Load thông tin khách hàng nếu có
                if (order.CustomerId.HasValue)
                {
                    customerId = order.CustomerId.Value;
                    Customer? customer = await hoaDonModels.GetCustomerByIdAsync(order.CustomerId.Value);
                    if (customer != null)
                    {
                        textBoxKhachHang.Text = customer.FullName;
                        textBoxSoDienThoai.Text = customer.PhoneNumber ?? "";
                        textBoxEmail.Text = customer.Email ?? "";
                    }
                }

                // Lấy danh sách chi tiết hóa đơn
                List<OrderDetail> orderDetails = await hoaDonModels.GetOrderDetailsByOrderIdAsync(orderId);

                if (orderDetails == null || orderDetails.Count == 0)
                {
                    // Không có chi tiết hóa đơn
                    orderDetailsTable.Rows.Clear();
                    CalculateTotal();
                    this.Cursor = Cursors.Default;
                    return;
                }

                // Lấy danh sách tất cả variant IDs cần thiết
                var variantIds = orderDetails.Select(d => d.VariantId).Distinct().ToList();

                // Load tất cả variant info một lần (tối ưu hơn)
                var variantDict = new Dictionary<int, ProductVariantInfo?>();
                foreach (var variantId in variantIds)
                {
                    try
                    {
                        var variant = await hoaDonModels.GetVariantInfoByIdAsync(variantId);
                        variantDict[variantId] = variant;
                    }
                    catch
                    {
                        // Nếu lỗi khi lấy variant, đánh dấu là null
                        variantDict[variantId] = null;
                    }
                }

                // Load chi tiết vào DataTable
                orderDetailsTable.Rows.Clear();
                foreach (var detail in orderDetails)
                {
                    // Lấy thông tin variant từ dictionary
                    variantDict.TryGetValue(detail.VariantId, out var variant);
                    
                    if (variant != null)
                    {
                        // Variant còn tồn tại - hiển thị thông tin đầy đủ
                        orderDetailsTable.Rows.Add(
                            detail.VariantId,
                            variant.SKU,
                            variant.ProductName,
                            variant.Size ?? "",
                            variant.Color ?? "",
                            detail.Quantity,
                            detail.UnitPrice,
                            detail.Subtotal
                        );
                    }
                    else
                    {
                        // Variant đã bị xóa - vẫn hiển thị với thông tin từ OrderDetails
                        orderDetailsTable.Rows.Add(
                            detail.VariantId,
                            $"SKU-{detail.VariantId}",
                            "Sản phẩm đã xóa",
                            "",
                            "",
                            detail.Quantity,
                            detail.UnitPrice,
                            detail.Subtotal
                        );
                    }
                }

                // Tính lại tổng tiền
                CalculateTotal();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Lỗi khi tải dữ liệu hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tìm khách hàng theo số điện thoại
        /// </summary>
        private async void TextBoxSoDienThoai_TextChanged(object sender, EventArgs e)
        {
            string phoneNumber = textBoxSoDienThoai.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(phoneNumber) || phoneNumber.Length < 10)
            {
                return;
            }

            try
            {
                Customer? customer = await hoaDonModels.GetCustomerByPhoneAsync(phoneNumber);
                if (customer != null)
                {
                    customerId = customer.CustomerId;
                    textBoxKhachHang.Text = customer.FullName;
                    textBoxEmail.Text = customer.Email ?? "";
                    // Có thể điền thêm địa chỉ nếu cần
                }
                else
                {
                    customerId = null;
                    if (string.IsNullOrWhiteSpace(textBoxKhachHang.Text))
                    {
                        textBoxKhachHang.Text = "";
                        textBoxEmail.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                // Không hiển thị lỗi khi đang gõ
            }
        }

        /// <summary>
        /// Thêm sản phẩm vào hóa đơn
        /// </summary>
        private void ButtonThemSanPham_Click(object sender, EventArgs e)
        {
            try
            {
                // Mở form chọn sản phẩm
                using (HoaDonChonSanPhamForm chonForm = new HoaDonChonSanPhamForm())
                {
                    if (chonForm.ShowDialog() == DialogResult.OK && chonForm.SelectedVariants != null && chonForm.SelectedVariants.Count > 0)
                    {
                        // Xử lý từng sản phẩm đã chọn
                        foreach (var variant in chonForm.SelectedVariants)
                        {
                            // Kiểm tra xem sản phẩm đã có trong danh sách chưa
                            bool exists = false;
                            foreach (DataRow row in orderDetailsTable.Rows)
                            {
                                if (Convert.ToInt32(row["VariantId"]) == variant.VariantId)
                                {
                                    // Tăng số lượng
                                    int currentQty = Convert.ToInt32(row["Số lượng"]);
                                    if (currentQty + 1 > variant.StockQuantity)
                                    {
                                        MessageBox.Show($"Sản phẩm '{variant.ProductName}' ({variant.Size}/{variant.Color}) vượt quá tồn kho (Tồn kho: {variant.StockQuantity}).", 
                                            "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        continue;
                                    }
                                    row["Số lượng"] = currentQty + 1;
                                    row["Thành tiền"] = (currentQty + 1) * variant.SellingPrice;
                                    exists = true;
                                    break;
                                }
                            }

                            if (!exists)
                            {
                                // Thêm mới
                                orderDetailsTable.Rows.Add(
                                    variant.VariantId, 
                                    variant.SKU, 
                                    variant.ProductName, 
                                    variant.Size ?? "", 
                                    variant.Color ?? "", 
                                    1, 
                                    variant.SellingPrice, 
                                    variant.SellingPrice
                                );
                            }
                        }

                        CalculateTotal();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm sản phẩm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xóa sản phẩm khỏi danh sách
        /// </summary>
        private void ButtonXoa_Click(object sender, EventArgs e)
        {
            if (dataGridViewSanPham.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridViewSanPham.SelectedRows)
                {
                    dataGridViewSanPham.Rows.Remove(row);
                }
                CalculateTotal();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Tính tổng tiền
        /// </summary>
        private void CalculateTotal()
        {
            decimal subtotal = 0;
            foreach (DataRow row in orderDetailsTable.Rows)
            {
                subtotal += Convert.ToDecimal(row["Thành tiền"]);
            }

            // Lấy giảm giá và tax
            decimal discount = 0;
            decimal tax = 0;

            if (decimal.TryParse(textBoxGiamGia.Text, out decimal discountValue))
            {
                discount = discountValue;
            }

            if (decimal.TryParse(textBoxTax.Text, out decimal taxValue))
            {
                tax = taxValue;
            }

            // Tổng tiền = Subtotal - Giảm giá + Tax
            decimal total = subtotal - discount + tax;

            textBoxTongTien.Text = total.ToString("N0");
        }

        /// <summary>
        /// Tính lại tổng khi giảm giá thay đổi
        /// </summary>
        private void TextBoxGiamGia_TextChanged(object sender, EventArgs e)
        {
            CalculateTotal();
        }

        /// <summary>
        /// Tính lại tổng khi tax thay đổi
        /// </summary>
        private void TextBoxTax_TextChanged(object sender, EventArgs e)
        {
            CalculateTotal();
        }

        /// <summary>
        /// Lưu hóa đơn
        /// </summary>
        private async void ButtonLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate
                if (orderDetailsTable.Rows.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm ít nhất một sản phẩm vào hóa đơn.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(labelMaHoaDon.Text))
                {
                    MessageBox.Show("Mã hóa đơn không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Lấy thông tin từ form
                decimal discount = 0;
                decimal tax = 0;
                decimal total = 0;

                // Parse số từ textbox (có thể có dấu phẩy do format N0)
                if (!decimal.TryParse(textBoxGiamGia.Text.Replace(",", ""), out discount))
                {
                    discount = 0;
                }

                if (!decimal.TryParse(textBoxTax.Text.Replace(",", ""), out tax))
                {
                    tax = 0;
                }

                if (!decimal.TryParse(textBoxTongTien.Text.Replace(",", ""), out total))
                {
                    MessageBox.Show("Tổng tiền không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Lấy User ID từ session
                int currentUserId = UserSession.GetCurrentUserId();
                if (currentUserId == 0)
                {
                    MessageBox.Show("Bạn chưa đăng nhập. Vui lòng đăng nhập lại.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tạo Order object
                Order order = new Order
                {
                    OrderCode = labelMaHoaDon.Text,
                    CustomerId = customerId,
                    UserId = currentUserId,
                    Tax = tax,
                    TotalAmount = total,
                    DiscountAmount = discount,
                    PaymentMethod = "Cash", // Có thể thêm ComboBox để chọn
                    Status = comboBox1.SelectedItem?.ToString() == "Đã thanh toán" ? "Completed" : 
                             comboBox1.SelectedItem?.ToString() == "Chưa thanh toán" ? "Pending" : "Pending",
                    ShippingAddress = textBox2.Text.Trim(),
                    BillingAddress = textBox1.Text.Trim(),
                    Note = "" // Có thể thêm TextBox cho ghi chú
                };

                if (isEditMode && orderId.HasValue)
                {
                    // Chế độ cập nhật
                    // Lấy trạng thái cũ trước khi cập nhật
                    Order? oldOrder = await hoaDonModels.GetOrderByIdAsync(orderId.Value);
                    string? oldStatus = oldOrder?.Status;

                    // Nếu status cũ là Completed, hoàn lại tồn kho trước khi xóa OrderDetails
                    // (Cần revert trước khi xóa OrderDetails để có dữ liệu chính xác)
                    if (oldStatus == "Completed")
                    {
                        await hoaDonModels.RevertOrderCompletionAsync(orderId.Value);
                    }

                    // Xóa tất cả OrderDetails cũ và thêm lại
                    await hoaDonModels.DeleteOrderDetailsAsync(orderId.Value);

                    // Thêm OrderDetails mới
                    List<OrderDetail> orderDetails = new List<OrderDetail>();
                    foreach (DataRow row in orderDetailsTable.Rows)
                    {
                        orderDetails.Add(new OrderDetail
                        {
                            OrderId = orderId.Value,
                            VariantId = Convert.ToInt32(row["VariantId"]),
                            Quantity = Convert.ToInt32(row["Số lượng"]),
                            UnitPrice = Convert.ToDecimal(row["Đơn giá"]),
                            Subtotal = Convert.ToDecimal(row["Thành tiền"])
                        });
                    }

                    await hoaDonModels.AddOrderDetailsAsync(orderDetails);

                    // Cập nhật Order
                    order.OrderId = orderId.Value;
                    await hoaDonModels.UpdateOrderAsync(order);

                    // Nếu status cũ VÀ mới đều là Completed, cần process lại dựa trên OrderDetails mới
                    // Vì UpdateOrderAsync chỉ xử lý khi status THAY ĐỔI, không xử lý khi status giữ nguyên
                    // Nhưng OrderDetails đã thay đổi, nên cần process lại
                    if (oldStatus == "Completed" && order.Status == "Completed")
                    {
                        await hoaDonModels.ProcessOrderCompletionAsync(orderId.Value);
                    }

                    MessageBox.Show("Cập nhật hóa đơn thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Chế độ thêm mới
                    int newOrderId = await hoaDonModels.AddOrderAsync(order);

                    // Thêm OrderDetails
                    List<OrderDetail> orderDetails = new List<OrderDetail>();
                    foreach (DataRow row in orderDetailsTable.Rows)
                    {
                        orderDetails.Add(new OrderDetail
                        {
                            OrderId = newOrderId,
                            VariantId = Convert.ToInt32(row["VariantId"]),
                            Quantity = Convert.ToInt32(row["Số lượng"]),
                            UnitPrice = Convert.ToDecimal(row["Đơn giá"]),
                            Subtotal = Convert.ToDecimal(row["Thành tiền"])
                        });
                    }

                    await hoaDonModels.AddOrderDetailsAsync(orderDetails);

                    // Nếu status là Completed, cập nhật tồn kho (giảm số lượng)
                    if (order.Status == "Completed")
                    {
                        await hoaDonModels.ProcessOrderCompletionAsync(newOrderId);
                    }

                    MessageBox.Show("Lưu hóa đơn thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
