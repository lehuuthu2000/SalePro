using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace helloworld
{
    public partial class SanPhamChiTietForm : Form
    {
        private ProductBLL sanPhamModels; // Đã chuyển sang ProductBLL
        private int productId; // ID sản phẩm cha
        private string? selectedImagePath = null;

        /// <summary>
        /// Constructor - Nhận productId để gán vào ProductVariant
        /// </summary>
        /// <param name="productId">ID sản phẩm cha</param>
        public SanPhamChiTietForm(int productId)
        {
            InitializeComponent();
            sanPhamModels = new ProductBLL();
            this.productId = productId;

            this.Load += SanPhamChiTietForm_Load;
            buttonLuu.Click += ButtonLuu_Click;
            pictureBox1.Click += PictureBox1_Click;
            pictureBox1.Cursor = Cursors.Hand;

            // Đặt tên controls cho dễ đọc
            // textBox1 = SKU
            // textBox2 = Size
            // textBox3 = Color
            // textBox4 = Import Price
            // textBox5 = Selling Price
            // textBox6 = Stock Quantity
            // pictureBox1 = Image
            // button1 = Save
        }

        /// <summary>
        /// Load form - Tự động tạo SKU
        /// </summary>
        private async void SanPhamChiTietForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Tự động tạo SKU (sẽ được cập nhật khi nhập size và color)
                textBox1.ReadOnly = true;
                textBox1.BackColor = Color.LightGray;
                textBox1.Text = "Đang tạo...";

                // Lấy product_code để tạo SKU
                string productCode = await sanPhamModels.GetProductCodeAsync(productId);
                textBox1.Text = productCode; // Hiển thị product_code ban đầu

                // Mặc định số lượng tồn kho = 0
                textBox6.Text = "0";

                // Thêm event handler để tự động cập nhật SKU khi thay đổi Size hoặc Color
                textBox2.TextChanged += (s, e) => UpdateSKU();
                textBox3.TextChanged += (s, e) => UpdateSKU();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi tạo form: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Cập nhật SKU khi thay đổi Size hoặc Color
        /// </summary>
        private async void UpdateSKU()
        {
            try
            {
                string productCode = await sanPhamModels.GetProductCodeAsync(productId);
                string size = textBox2.Text.Trim();
                string color = textBox3.Text.Trim();

                // Tạo SKU tự động
                string sku = productCode;
                if (!string.IsNullOrWhiteSpace(size))
                {
                    // Loại bỏ khoảng trắng trong size
                    string sizeCode = size.ToUpper().Replace(" ", "");
                    sku += $"-{sizeCode}";
                }
                if (!string.IsNullOrWhiteSpace(color))
                {
                    // Chuyển tiếng Việt có dấu thành không dấu, viết hoa và loại bỏ khoảng trắng
                    string colorCode = RemoveVietnameseAccents(color.ToUpper());
                    colorCode = colorCode.Replace(" ", ""); // Loại bỏ tất cả khoảng trắng
                    sku += $"-{colorCode}";
                }

                textBox1.Text = sku;
            }
            catch (Exception ex)
            {
                // Không hiển thị lỗi khi cập nhật SKU
            }
        }

        /// <summary>
        /// Loại bỏ dấu tiếng Việt (copy từ SanPhamModels)
        /// </summary>
        private string RemoveVietnameseAccents(string text)
        {
            string[] vietnameseChars = { "à", "á", "ạ", "ả", "ã", "â", "ầ", "ấ", "ậ", "ẩ", "ẫ", "ă", "ằ", "ắ", "ặ", "ẳ", "ẵ",
                                         "è", "é", "ẹ", "ẻ", "ẽ", "ê", "ề", "ế", "ệ", "ể", "ễ",
                                         "ì", "í", "ị", "ỉ", "ĩ",
                                         "ò", "ó", "ọ", "ỏ", "õ", "ô", "ồ", "ố", "ộ", "ổ", "ỗ", "ơ", "ờ", "ớ", "ợ", "ở", "ỡ",
                                         "ù", "ú", "ụ", "ủ", "ũ", "ư", "ừ", "ứ", "ự", "ử", "ữ",
                                         "ỳ", "ý", "ỵ", "ỷ", "ỹ",
                                         "đ" };

            string[] replacementChars = { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                                          "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e",
                                          "i", "i", "i", "i", "i",
                                          "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o",
                                          "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u",
                                          "y", "y", "y", "y", "y",
                                          "d" };

            string result = text;
            for (int i = 0; i < vietnameseChars.Length; i++)
            {
                result = result.Replace(vietnameseChars[i], replacementChars[i]);
                result = result.Replace(vietnameseChars[i].ToUpper(), replacementChars[i].ToUpper());
            }

            return result;
        }

        /// <summary>
        /// Xử lý khi click vào pictureBox để chọn ảnh
        /// </summary>
        private void PictureBox1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        selectedImagePath = openFileDialog.FileName;

                        // Hiển thị ảnh preview
                        if (File.Exists(selectedImagePath))
                        {
                            Image image = Image.FromFile(selectedImagePath);
                            pictureBox1.Image = image;
                            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi tải ảnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Lưu ProductVariant
        /// </summary>
        private async void ButtonLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("SKU không được để trống.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.Focus();
                    return;
                }

                // Parse các giá trị số
                decimal importPrice = 0;
                if (!string.IsNullOrWhiteSpace(textBox4.Text))
                {
                    if (!decimal.TryParse(textBox4.Text, out importPrice))
                    {
                        MessageBox.Show("Giá nhập không hợp lệ.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox4.Focus();
                        return;
                    }
                }

                decimal sellingPrice = 0;
                if (!string.IsNullOrWhiteSpace(textBox5.Text))
                {
                    if (!decimal.TryParse(textBox5.Text, out sellingPrice))
                    {
                        MessageBox.Show("Giá bán không hợp lệ.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox5.Focus();
                        return;
                    }
                }

                int stockQuantity = 0;
                if (!string.IsNullOrWhiteSpace(textBox6.Text))
                {
                    if (!int.TryParse(textBox6.Text, out stockQuantity))
                    {
                        MessageBox.Show("Số lượng tồn kho không hợp lệ.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox6.Focus();
                        return;
                    }
                }

                // Lấy dữ liệu từ form
                string? sku = textBox1.Text.Trim();
                string? size = string.IsNullOrWhiteSpace(textBox2.Text) ? null : textBox2.Text.Trim();
                string? color = string.IsNullOrWhiteSpace(textBox3.Text) ? null : textBox3.Text.Trim();
                string? imagePath = selectedImagePath;
                bool isActive = true; // Mặc định là hoạt động

                // Thêm ProductVariant
                int variantId = await sanPhamModels.AddProductVariantAsync(
                    productId,
                    sku,
                    size,
                    color,
                    importPrice,
                    sellingPrice,
                    stockQuantity,
                    imagePath,
                    isActive
                );

                MessageBox.Show(
                    $"Thêm biến thể sản phẩm thành công!\nSKU: {sku}\nVariant ID: {variantId}",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
             }
        }
    }
}
