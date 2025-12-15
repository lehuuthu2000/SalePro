using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace helloworld
{
    public partial class KhachHangForm : Form
    {
        private CustomerBLL khachHangModels; // Đã chuyển sang CustomerBLL
        private OrderBLL hoaDonModels; // Để mở chi tiết hóa đơn
        private int? editingCustomerId = null; // ID khách hàng đang chỉnh sửa (null nếu là Add mode)
        private byte[]? selectedImageBytes = null;
        private byte[]? originalImageBytes = null; // Ảnh gốc từ database (để giữ nguyên nếu không thay đổi)
        
        // DataBinding cho hóa đơn
        private BindingSource ordersBindingSource = new BindingSource();
        private DataTable ordersDataTable;
        private DataView ordersDataView;

        /// <summary>
        /// Constructor cho Add mode (thêm mới khách hàng)
        /// </summary>
        public KhachHangForm() : this(null)
        {
        }

        /// <summary>
        /// Constructor với customerId - nếu có thì là Edit mode, không có thì là Add mode
        /// </summary>
        /// <param name="customerId">ID khách hàng cần chỉnh sửa (null nếu thêm mới)</param>
        public KhachHangForm(int? customerId)
        {
            InitializeComponent();
            khachHangModels = new CustomerBLL(); // Đã chuyển sang CustomerBLL
            hoaDonModels = new OrderBLL(); // Để mở chi tiết hóa đơn
            editingCustomerId = customerId;
            
            // Khởi tạo DataBinding cho hóa đơn
            dataGridViewHoaDonKhachHang.DataSource = ordersBindingSource;
            
            buttonLuu.Click += ButtonLuu_Click;
            pictureBoxImage.Click += PictureBoxImage_Click;
            pictureBoxPhone.Click += PictureBoxPhone_Click;
            pictureBoxEmail.Click += PictureBoxEmail_Click;
            
            // Thiết lập PictureBox để hiển thị ảnh đúng cách
            pictureBoxImage.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxImage.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxImage.BackColor = Color.White;
            
            // Thiết lập cursor cho pictureBoxPhone và pictureBoxEmail để hiển thị là có thể click
            pictureBoxPhone.Cursor = Cursors.Hand;
            pictureBoxEmail.Cursor = Cursors.Hand;

            // Thiết lập DataGridView cho hóa đơn
            ConfigureOrdersDataGridView();
            
            // Event handler cho double-click để mở chi tiết hóa đơn
            dataGridViewHoaDonKhachHang.CellDoubleClick += DataGridViewHoaDonKhachHang_CellDoubleClick;

            // Nếu là Edit mode, load dữ liệu khách hàng
            if (editingCustomerId.HasValue)
            {
                this.Text = "Thông tin khách hàng";
                this.Load += KhachHangForm_Load;
            }
            else
            {
                this.Text = "Thêm mới khách hàng";
            }
        }

        /// <summary>
        /// Load dữ liệu khách hàng khi form được mở ở Edit mode
        /// </summary>
        private async void KhachHangForm_Load(object sender, EventArgs e)
        {
            if (editingCustomerId.HasValue)
            {
                await LoadCustomerDataAsync(editingCustomerId.Value);
                await LoadCustomerOrdersAsync(editingCustomerId.Value);
            }
        }

        /// <summary>
        /// Load dữ liệu khách hàng từ database để hiển thị trong form
        /// </summary>
        private async Task LoadCustomerDataAsync(int customerId)
        {
            try
            {
                // Hiển thị loading
                buttonLuu.Enabled = false;
                buttonLuu.Text = "Đang tải...";

                Customer customer = await khachHangModels.GetCustomerByIdAsync(customerId);

                // Điền dữ liệu vào các textbox
                textBoxHoTen.Text = customer.FullName;
                textBoxSoDienThoai.Text = customer.PhoneNumber ?? string.Empty;
                textBoxEmail.Text = customer.Email ?? string.Empty;
                textBoxDiaChi.Text = customer.Address ?? string.Empty;
                textBoxDiemTichLuy.Text = customer.Points.ToString();

                // Xử lý ảnh
                if (customer.Image != null && customer.Image.Length > 0)
                {
                    originalImageBytes = customer.Image;
                    selectedImageBytes = customer.Image; // Mặc định giữ nguyên ảnh cũ
                    
                    // Hiển thị ảnh trong PictureBox
                    using (MemoryStream ms = new MemoryStream(customer.Image))
                    {
                        pictureBoxImage.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    pictureBoxImage.Image = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi tải thông tin khách hàng: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                this.Close();
            }
            finally
            {
                buttonLuu.Enabled = true;
                buttonLuu.Text = "Lưu";
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Lưu - Thêm mới hoặc Cập nhật khách hàng
        /// </summary>
        private async void ButtonLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate dữ liệu từ form
                if (string.IsNullOrWhiteSpace(textBoxHoTen.Text))
                {
                    MessageBox.Show(
                        "Vui lòng nhập họ và tên.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    textBoxHoTen.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBoxSoDienThoai.Text))
                {
                    MessageBox.Show(
                        "Vui lòng nhập số điện thoại.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    textBoxSoDienThoai.Focus();
                    return;
                }

                // Xử lý điểm tích lũy
                int points = 0;
                if (!string.IsNullOrWhiteSpace(textBoxDiemTichLuy.Text))
                {
                    if (!int.TryParse(textBoxDiemTichLuy.Text.Trim(), out points))
                    {
                        MessageBox.Show(
                            "Điểm tích lũy phải là số nguyên.",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        textBoxDiemTichLuy.Focus();
                        return;
                    }
                }

                // Xử lý ảnh: Nếu không chọn ảnh mới và đang ở Edit mode, giữ nguyên ảnh cũ
                byte[]? imageToSave = selectedImageBytes;
                if (editingCustomerId.HasValue && selectedImageBytes == null && originalImageBytes != null)
                {
                    // Đang edit và không chọn ảnh mới, giữ nguyên ảnh cũ
                    imageToSave = originalImageBytes;
                }

                // Tạo đối tượng Customer từ dữ liệu form
                Customer customer = new Customer
                {
                    FullName = textBoxHoTen.Text.Trim(),
                    PhoneNumber = textBoxSoDienThoai.Text.Trim(),
                    Email = string.IsNullOrWhiteSpace(textBoxEmail.Text) ? null : textBoxEmail.Text.Trim(),
                    Address = string.IsNullOrWhiteSpace(textBoxDiaChi.Text) ? null : textBoxDiaChi.Text.Trim(),
                    Image = imageToSave,
                    Points = points
                };

                // Disable nút Lưu để tránh click nhiều lần
                buttonLuu.Enabled = false;
                buttonLuu.Text = "Đang lưu...";

                if (editingCustomerId.HasValue)
                {
                    // Edit mode: Cập nhật khách hàng
                    customer.CustomerId = editingCustomerId.Value;
                    await khachHangModels.UpdateCustomerAsync(customer);

                    // Hiển thị thông báo thành công
                    MessageBox.Show(
                        $"Cập nhật khách hàng thành công!",
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    // Add mode: Thêm khách hàng mới
                    int customerId = await khachHangModels.AddCustomerAsync(customer);

                    // Hiển thị thông báo thành công
                    MessageBox.Show(
                        $"Thêm khách hàng thành công!\nMã khách hàng: {customerId}",
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                // Đóng form sau khi lưu thành công
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (ArgumentException ex)
            {
                // Lỗi validation
                MessageBox.Show(
                    ex.Message,
                    "Lỗi dữ liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                buttonLuu.Enabled = true;
                buttonLuu.Text = "Lưu";
            }
            catch (Exception ex)
            {
                // Lỗi khác (database, network, etc.)
                MessageBox.Show(
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                buttonLuu.Enabled = true;
                buttonLuu.Text = "Lưu";
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi click vào PictureBox để chọn ảnh
        /// </summary>
        private void PictureBoxImage_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.Title = "Chọn ảnh khách hàng";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;

                        // Kiểm tra kích thước file (giới hạn 5MB)
                        FileInfo fileInfo = new FileInfo(filePath);
                        if (fileInfo.Length > 5 * 1024 * 1024) // 5MB
                        {
                            MessageBox.Show(
                                "Kích thước ảnh không được vượt quá 5MB.",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }

                        // Đọc ảnh vào byte array
                        selectedImageBytes = File.ReadAllBytes(filePath);

                        // Hiển thị ảnh trong PictureBox
                        using (MemoryStream ms = new MemoryStream(selectedImageBytes))
                        {
                            // Dispose ảnh cũ trước khi gán ảnh mới
                            if (pictureBoxImage.Image != null)
                            {
                                pictureBoxImage.Image.Dispose();
                            }
                            pictureBoxImage.Image = Image.FromStream(ms);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi chọn ảnh: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi click vào pictureBoxPhone - Gọi điện trực tiếp
        /// </summary>
        private void PictureBoxPhone_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy số điện thoại từ textBox
                string phoneNumber = textBoxSoDienThoai.Text?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    MessageBox.Show(
                        "Khách hàng chưa có số điện thoại.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                // Loại bỏ các ký tự không phải số (giữ lại + nếu có)
                string cleanPhoneNumber = Regex.Replace(phoneNumber, @"[^\d+]", "");

                if (string.IsNullOrWhiteSpace(cleanPhoneNumber))
                {
                    MessageBox.Show(
                        "Số điện thoại không hợp lệ.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // Sử dụng tel: protocol để mở ứng dụng gọi điện trực tiếp
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"tel:{cleanPhoneNumber}",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể mở ứng dụng gọi điện: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi click vào pictureBoxEmail - Mở email client
        /// </summary>
        private void PictureBoxEmail_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy email từ textBox
                string email = textBoxEmail.Text?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show(
                        "Khách hàng chưa có email.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                // Validate email format
                if (!IsValidEmail(email))
                {
                    MessageBox.Show(
                        "Email không hợp lệ.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // Mở email client với mailto: protocol
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"mailto:{email}",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể mở email client: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Kiểm tra định dạng email có hợp lệ không
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Load danh sách hóa đơn của khách hàng
        /// </summary>
        private async Task LoadCustomerOrdersAsync(int customerId)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                
                // Lấy danh sách hóa đơn từ BLL
                ordersDataTable = await khachHangModels.GetCustomerOrdersDataTableAsync(customerId);
                
                // Tạo DataView để hỗ trợ tìm kiếm và sắp xếp
                ordersDataView = ordersDataTable.DefaultView;
                
                // Bind vào BindingSource
                ordersBindingSource.DataSource = ordersDataView;
                
                // Đảm bảo DataGridView được bind đúng
                if (dataGridViewHoaDonKhachHang.DataSource != ordersBindingSource)
                {
                    dataGridViewHoaDonKhachHang.DataSource = ordersBindingSource;
                }
                
                // Cấu hình lại DataGridView
                ConfigureOrdersDataGridView();
                
                // Refresh DataGridView
                dataGridViewHoaDonKhachHang.Refresh();
                
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(
                    $"Lỗi khi tải danh sách hóa đơn: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Cấu hình DataGridView cho hóa đơn
        /// </summary>
        private void ConfigureOrdersDataGridView()
        {
            dataGridViewHoaDonKhachHang.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewHoaDonKhachHang.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewHoaDonKhachHang.ReadOnly = true;
            dataGridViewHoaDonKhachHang.RowHeadersVisible = false;
            dataGridViewHoaDonKhachHang.AllowUserToAddRows = false;
            dataGridViewHoaDonKhachHang.MultiSelect = false;

            // Format các cột nếu đã có dữ liệu
            if (dataGridViewHoaDonKhachHang.Columns.Count > 0)
            {
                // Format cột Tổng tiền
                if (dataGridViewHoaDonKhachHang.Columns["Tổng tiền"] != null)
                {
                    dataGridViewHoaDonKhachHang.Columns["Tổng tiền"].DefaultCellStyle.Format = "N0";
                    dataGridViewHoaDonKhachHang.Columns["Tổng tiền"].DefaultCellStyle.Alignment = 
                        DataGridViewContentAlignment.MiddleRight;
                }

                // Format cột Ngày tạo
                if (dataGridViewHoaDonKhachHang.Columns["Ngày tạo"] != null)
                {
                    dataGridViewHoaDonKhachHang.Columns["Ngày tạo"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    dataGridViewHoaDonKhachHang.Columns["Ngày tạo"].DefaultCellStyle.Alignment = 
                        DataGridViewContentAlignment.MiddleCenter;
                }

                // Format cột Trạng thái
                if (dataGridViewHoaDonKhachHang.Columns["Trạng thái"] != null)
                {
                    dataGridViewHoaDonKhachHang.Columns["Trạng thái"].DefaultCellStyle.Alignment = 
                        DataGridViewContentAlignment.MiddleCenter;
                }

                // Format cột Phương thức thanh toán
                if (dataGridViewHoaDonKhachHang.Columns["Phương thức thanh toán"] != null)
                {
                    dataGridViewHoaDonKhachHang.Columns["Phương thức thanh toán"].DefaultCellStyle.Alignment = 
                        DataGridViewContentAlignment.MiddleCenter;
                }

                // Ẩn cột Mã HĐ (chỉ dùng để xử lý)
                if (dataGridViewHoaDonKhachHang.Columns["Mã HĐ"] != null)
                {
                    dataGridViewHoaDonKhachHang.Columns["Mã HĐ"].Visible = false;
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện double-click vào hóa đơn để mở chi tiết
        /// </summary>
        private async void DataGridViewHoaDonKhachHang_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                // Lấy Order ID từ bindingSource
                DataRowView rowView = (DataRowView)dataGridViewHoaDonKhachHang.Rows[e.RowIndex].DataBoundItem;
                
                if (rowView["Mã HĐ"] != null && 
                    int.TryParse(rowView["Mã HĐ"].ToString(), out int orderId))
                {
                    // Mở form chi tiết hóa đơn
                    HoaDonForm hoaDonForm = new HoaDonForm(orderId);
                    hoaDonForm.ShowDialog();
                    
                    // Reload danh sách hóa đơn sau khi đóng form (nếu có thay đổi)
                    if (hoaDonForm.DialogResult == DialogResult.OK && editingCustomerId.HasValue)
                    {
                        await LoadCustomerOrdersAsync(editingCustomerId.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi mở chi tiết hóa đơn: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
