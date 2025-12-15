using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace helloworld
{
    /// <summary>
    /// Data Access Layer cho Users
    /// Chỉ chứa các thao tác database, không có business logic
    /// </summary>
    internal class UserDAL
    {
        private ConnectDatabase database;

        public UserDAL()
        {
            database = new ConnectDatabase();
        }

        /// <summary>
        /// Kiểm tra xem username đã tồn tại chưa
        /// </summary>
        public async Task<bool> CheckUsernameExistsAsync(string username)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = "SELECT COUNT(*) FROM Users WHERE username = @username";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username.Trim());

                        object? result = await command.ExecuteScalarAsync();
                        return result != null && Convert.ToInt32(result) > 0;
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra tên đăng nhập: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm user mới vào database
        /// </summary>
        public async Task RegisterUserAsync(string username, string password, string fullName, string? email = null)
        {
            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        INSERT INTO Users (username, password, full_name, email, is_admin, is_active)
                        VALUES (@username, @password, @full_name, @email, FALSE, TRUE)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username.Trim());
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@full_name", fullName.Trim());
                        command.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(email) ? DBNull.Value : email.Trim());
                        
                        await command.ExecuteNonQueryAsync();
                    }
                });
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                {
                    throw new Exception($"Tên đăng nhập đã tồn tại trong hệ thống.", ex);
                }
                throw new Exception($"Lỗi khi đăng ký: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đăng ký: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm user theo username
        /// </summary>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT user_id, username, full_name, email, is_admin, is_active
                        FROM Users
                        WHERE username = @username AND is_active = TRUE";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username.Trim());

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int userIdIndex = reader.GetOrdinal("user_id");
                                int usernameIndex = reader.GetOrdinal("username");
                                int fullNameIndex = reader.GetOrdinal("full_name");
                                int emailIndex = reader.GetOrdinal("email");
                                int isAdminIndex = reader.GetOrdinal("is_admin");
                                int isActiveIndex = reader.GetOrdinal("is_active");

                                User user = new User
                                {
                                    UserId = reader.GetInt32(userIdIndex),
                                    Username = reader.GetString(usernameIndex),
                                    FullName = reader.GetString(fullNameIndex),
                                    Email = reader.IsDBNull(emailIndex) ? null : reader.GetString(emailIndex),
                                    IsAdmin = reader.GetInt32(isAdminIndex) != 0,
                                    IsActive = reader.GetInt32(isActiveIndex) != 0
                                };

                                return user;
                            }
                        }
                    }
                    return null;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm user theo username: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm user theo email
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT user_id, username, full_name, email, is_admin, is_active
                        FROM Users
                        WHERE email = @email AND is_active = TRUE";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@email", email.Trim());

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int userIdIndex = reader.GetOrdinal("user_id");
                                int usernameIndex = reader.GetOrdinal("username");
                                int fullNameIndex = reader.GetOrdinal("full_name");
                                int emailIndex = reader.GetOrdinal("email");
                                int isAdminIndex = reader.GetOrdinal("is_admin");
                                int isActiveIndex = reader.GetOrdinal("is_active");

                                User user = new User
                                {
                                    UserId = reader.GetInt32(userIdIndex),
                                    Username = reader.GetString(usernameIndex),
                                    FullName = reader.GetString(fullNameIndex),
                                    Email = reader.IsDBNull(emailIndex) ? null : reader.GetString(emailIndex),
                                    IsAdmin = reader.GetInt32(isAdminIndex) != 0,
                                    IsActive = reader.GetInt32(isActiveIndex) != 0
                                };

                                return user;
                            }
                        }
                    }
                    return null;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm user theo email: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật mật khẩu cho user
        /// </summary>
        public async Task UpdatePasswordAsync(int userId, string newPassword)
        {
            try
            {
                await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        UPDATE Users
                        SET password = @password, updated_at = NOW()
                        WHERE user_id = @user_id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@password", newPassword);
                        command.Parameters.AddWithValue("@user_id", userId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            throw new Exception("Không tìm thấy user để cập nhật mật khẩu.");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật mật khẩu: {ex.Message}", ex);
            }
        }
    }
}

