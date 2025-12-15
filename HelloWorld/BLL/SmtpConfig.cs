using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace helloworld
{
    /// <summary>
    /// Class quản lý cấu hình SMTP một cách an toàn
    /// Sử dụng Windows DPAPI để mã hóa mật khẩu
    /// </summary>
    internal class SmtpConfig
    {
        private static readonly string ConfigFilePath = Path.Combine(
            Application.UserAppDataPath,
            "smtp_config.dat"
        );

        public string SmtpHost { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;

        /// <summary>
        /// Kiểm tra xem đã có cấu hình SMTP chưa
        /// </summary>
        public bool IsConfigured()
        {
            return File.Exists(ConfigFilePath) && 
                   !string.IsNullOrWhiteSpace(SmtpUsername) && 
                   !string.IsNullOrWhiteSpace(SmtpPassword);
        }

        /// <summary>
        /// Load cấu hình SMTP từ file (đã mã hóa)
        /// </summary>
        public void Load()
        {
            try
            {
                if (!File.Exists(ConfigFilePath))
                {
                    // Nếu chưa có file config, sử dụng giá trị mặc định
                    return;
                }

                string[] lines = File.ReadAllLines(ConfigFilePath);
                if (lines.Length >= 4)
                {
                    SmtpHost = DecryptString(lines[0]);
                    SmtpPort = int.Parse(DecryptString(lines[1]));
                    SmtpUsername = DecryptString(lines[2]);
                    SmtpPassword = DecryptString(lines[3]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đọc cấu hình SMTP: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lưu cấu hình SMTP vào file (mã hóa)
        /// </summary>
        public void Save()
        {
            try
            {
                // Đảm bảo thư mục tồn tại
                string directory = Path.GetDirectoryName(ConfigFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string[] lines = new string[]
                {
                    EncryptString(SmtpHost),
                    EncryptString(SmtpPort.ToString()),
                    EncryptString(SmtpUsername),
                    EncryptString(SmtpPassword)
                };

                File.WriteAllLines(ConfigFilePath, lines);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu cấu hình SMTP: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Mã hóa string sử dụng Windows DPAPI (Data Protection API)
        /// Chỉ có thể giải mã trên cùng một máy và cùng user account
        /// </summary>
        private string EncryptString(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            try
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = ProtectedData.Protect(
                    plainBytes,
                    null, // Optional entropy (additional key)
                    DataProtectionScope.CurrentUser // Chỉ user hiện tại mới giải mã được
                );
                return Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi mã hóa: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Giải mã string sử dụng Windows DPAPI
        /// </summary>
        private string DecryptString(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return string.Empty;

            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                byte[] plainBytes = ProtectedData.Unprotect(
                    encryptedBytes,
                    null, // Optional entropy
                    DataProtectionScope.CurrentUser
                );
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi giải mã: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa file cấu hình (để reset)
        /// </summary>
        public void Delete()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    File.Delete(ConfigFilePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa cấu hình: {ex.Message}", ex);
            }
        }
    }
}

