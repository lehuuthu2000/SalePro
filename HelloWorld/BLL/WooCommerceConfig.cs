using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace helloworld.BLL
{
    /// <summary>
    /// Class quản lý cấu hình kết nối WooCommerce một cách an toàn
    /// Sử dụng Windows DPAPI để mã hóa API Keys
    /// </summary>
    public class WooCommerceConfig
    {
        private static readonly string ConfigFilePath = Path.Combine(
            Application.UserAppDataPath,
            "woocommerce_config.dat"
        );

        public string Domain { get; set; } = string.Empty;
        public string ConsumerKey { get; set; } = string.Empty;
        public string ConsumerSecret { get; set; } = string.Empty;

        /// <summary>
        /// Kiểm tra xem đã có cấu hình chưa
        /// </summary>
        public bool IsConfigured()
        {
            return File.Exists(ConfigFilePath) &&
                   !string.IsNullOrWhiteSpace(Domain) &&
                   !string.IsNullOrWhiteSpace(ConsumerKey) &&
                   !string.IsNullOrWhiteSpace(ConsumerSecret);
        }

        /// <summary>
        /// Load cấu hình từ file (đã mã hóa)
        /// </summary>
        public void Load()
        {
            try
            {
                if (!File.Exists(ConfigFilePath))
                {
                    return;
                }

                string[] lines = File.ReadAllLines(ConfigFilePath);
                if (lines.Length >= 3)
                {
                    Domain = DecryptString(lines[0]);
                    ConsumerKey = DecryptString(lines[1]);
                    ConsumerSecret = DecryptString(lines[2]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đọc cấu hình WooCommerce: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lưu cấu hình vào file (mã hóa)
        /// </summary>
        public void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(ConfigFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string[] lines = new string[]
                {
                    EncryptString(Domain),
                    EncryptString(ConsumerKey),
                    EncryptString(ConsumerSecret)
                };

                File.WriteAllLines(ConfigFilePath, lines);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu cấu hình WooCommerce: {ex.Message}", ex);
            }
        }

        private string EncryptString(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            try
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = ProtectedData.Protect(
                    plainBytes, null, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception ex) { throw new Exception($"Lỗi mã hóa: {ex.Message}", ex); }
        }

        private string DecryptString(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText)) return string.Empty;
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                byte[] plainBytes = ProtectedData.Unprotect(
                    encryptedBytes, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (Exception ex) { throw new Exception($"Lỗi giải mã: {ex.Message}", ex); }
        }

        public void Delete()
        {
            if (File.Exists(ConfigFilePath)) File.Delete(ConfigFilePath);
        }
    }
}
