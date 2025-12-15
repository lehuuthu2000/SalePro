using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace helloworld
{
    /// <summary>
    /// Business Logic Layer cho Authentication
    /// Chứa business logic, validation, và gọi AuthDAL và UserDAL
    /// Giữ nguyên interface public như DangNhapModels và DangKyModels cũ để tương thích với Forms
    /// </summary>
    internal class AuthBLL
    {
        private AuthDAL authDAL;
        private UserDAL userDAL;

        public AuthBLL()
        {
            authDAL = new AuthDAL();
            userDAL = new UserDAL();
        }

        /// <summary>
        /// Xác thực đăng nhập
        /// </summary>
        public async Task<User?> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            try
            {
                var (user, storedPassword) = await authDAL.GetUserForLoginAsync(username);
                
                if (user == null || storedPassword == null)
                {
                    return null;
                }

                // Xác minh mật khẩu (tạm thời so sánh trực tiếp, nên thay bằng hash trong production)
                if (VerifyPassword(password, storedPassword))
                {
                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đăng nhập: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xác minh mật khẩu (tạm thời so sánh trực tiếp, nên thay bằng hash trong production)
        /// </summary>
        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            // Tạm thời so sánh trực tiếp (không an toàn cho production)
            // Trong production nên sử dụng BCrypt hoặc SHA256
            return inputPassword == storedPassword;
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        public async Task RegisterAsync(string username, string password, string? fullName = null, string? email = null)
        {
            // Validate username
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Tên đăng nhập không được để trống.");
            }

            if (username.Length < 3)
            {
                throw new ArgumentException("Tên đăng nhập phải có ít nhất 3 ký tự.");
            }

            if (username.Length > 50)
            {
                throw new ArgumentException("Tên đăng nhập không được vượt quá 50 ký tự.");
            }

            // Kiểm tra ký tự đặc biệt trong username
            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
            {
                throw new ArgumentException("Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới (_).");
            }

            // Validate password
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Mật khẩu không được để trống.");
            }

            if (password.Length < 6)
            {
                throw new ArgumentException("Mật khẩu phải có ít nhất 6 ký tự.");
            }

            if (password.Length > 100)
            {
                throw new ArgumentException("Mật khẩu không được vượt quá 100 ký tự.");
            }

            // Validate email nếu có
            if (!string.IsNullOrWhiteSpace(email))
            {
                if (email.Length > 100)
                {
                    throw new ArgumentException("Email không được vượt quá 100 ký tự.");
                }

                // Kiểm tra định dạng email
                if (!IsValidEmail(email))
                {
                    throw new ArgumentException("Email không hợp lệ.");
                }
            }

            // Kiểm tra username đã tồn tại chưa
            if (await userDAL.CheckUsernameExistsAsync(username))
            {
                throw new Exception($"Tên đăng nhập '{username}' đã tồn tại. Vui lòng chọn tên khác.");
            }

            if (string.IsNullOrWhiteSpace(fullName))
            {
                fullName = username;
            }

            await userDAL.RegisterUserAsync(username, password, fullName, email);
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
        /// Xử lý quên mật khẩu: Tìm user theo username hoặc email, tạo mật khẩu mới, gửi email và cập nhật database
        /// </summary>
        public async Task ForgotPasswordAsync(string username, string email)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Vui lòng nhập tên đăng nhập hoặc địa chỉ email.");
            }

            username = username?.Trim() ?? string.Empty;
            email = email?.Trim() ?? string.Empty;

            // Validate email nếu có
            if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
            {
                throw new ArgumentException("Địa chỉ email không hợp lệ.");
            }

            try
            {
                User? user = null;

                // Tìm user theo username hoặc email
                if (!string.IsNullOrWhiteSpace(username))
                {
                    user = await userDAL.GetUserByUsernameAsync(username);
                }

                // Nếu không tìm thấy theo username, thử tìm theo email
                if (user == null && !string.IsNullOrWhiteSpace(email))
                {
                    user = await userDAL.GetUserByEmailAsync(email);
                }

                if (user == null)
                {
                    // Không thông báo chi tiết để tránh lộ thông tin
                    throw new Exception("Nếu thông tin này tồn tại trong hệ thống, chúng tôi đã gửi mật khẩu mới đến email của bạn.");
                }

                // Kiểm tra user có email không
                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    throw new Exception("Tài khoản này chưa có địa chỉ email. Vui lòng liên hệ quản trị viên để được hỗ trợ.");
                }

                // Tạo mật khẩu mới ngẫu nhiên
                string newPassword = GenerateRandomPassword(12);

                // Gửi email chứa mật khẩu mới
                await SendPasswordResetEmailAsync(user.Email, user.FullName, user.Username, newPassword);

                // Cập nhật mật khẩu mới vào database
                await userDAL.UpdatePasswordAsync(user.UserId, newPassword);
            }
            catch (ArgumentException)
            {
                throw; // Re-throw validation errors
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xử lý quên mật khẩu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tạo mật khẩu ngẫu nhiên
        /// </summary>
        private string GenerateRandomPassword(int length)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            StringBuilder password = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }

            return password.ToString();
        }

        /// <summary>
        /// Gửi email chứa mật khẩu mới qua SMTP
        /// Sử dụng cấu hình SMTP từ form Cài đặt (ViewsCaiDat)
        /// </summary>
        private async Task SendPasswordResetEmailAsync(string toEmail, string fullName, string username, string newPassword)
        {
            try
            {
                // Load cấu hình SMTP từ file đã mã hóa
                SmtpConfig smtpConfig = new SmtpConfig();
                smtpConfig.Load();

                // Kiểm tra xem đã có cấu hình chưa
                if (!smtpConfig.IsConfigured())
                {
                    throw new Exception(
                        "Chưa cấu hình SMTP. Vui lòng vào Cài đặt để cấu hình thông tin SMTP.\n\n" +
                        "HƯỚNG DẪN CẤU HÌNH:\n" +
                        "1. Gmail: smtp.gmail.com, Port 587\n" +
                        "2. Outlook: smtp-mail.outlook.com, Port 587\n" +
                        "3. Yahoo: smtp.mail.yahoo.com, Port 587\n\n" +
                        "Lưu ý: Với Gmail, cần tạo App Password từ Google Account > Security > 2-Step Verification > App passwords"
                    );
                }

                // Tạo email message
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(smtpConfig.SmtpUsername, "Hệ thống Quản lý Bán hàng");
                mail.To.Add(new MailAddress(toEmail, fullName));
                mail.Subject = "Cấp lại mật khẩu - Hệ thống Quản lý Bán hàng";
                mail.Body = $@"
Xin chào {fullName},

Bạn đã yêu cầu cấp lại mật khẩu cho tài khoản của bạn.

Thông tin tài khoản:
- Tên đăng nhập: {username}
- Mật khẩu mới: {newPassword}

Vui lòng đăng nhập và thay đổi mật khẩu ngay sau khi nhận được email này để đảm bảo an toàn.

Lưu ý: Vui lòng không chia sẻ mật khẩu với bất kỳ ai.

Nếu bạn không yêu cầu cấp lại mật khẩu, vui lòng liên hệ với quản trị viên ngay lập tức.

Trân trọng,
Hệ thống Quản lý Bán hàng
";
                mail.IsBodyHtml = false;
                mail.BodyEncoding = Encoding.UTF8;
                mail.SubjectEncoding = Encoding.UTF8;

                // Cấu hình SMTP client
                SmtpClient smtpClient = new SmtpClient(smtpConfig.SmtpHost, smtpConfig.SmtpPort);
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(smtpConfig.SmtpUsername, smtpConfig.SmtpPassword);
                smtpClient.Timeout = 30000; // 30 seconds

                // Gửi email bất đồng bộ
                await Task.Run(() => smtpClient.Send(mail));

                // Dispose resources
                mail.Dispose();
                smtpClient.Dispose();
            }
            catch (SmtpException ex)
            {
                throw new Exception($"Lỗi khi gửi email: {ex.Message}. Vui lòng kiểm tra cấu hình SMTP.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi gửi email: {ex.Message}", ex);
            }
        }
    }
}

