using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using helloworld.DAL;

namespace helloworld
{
    /// <summary>
    /// Data Access Layer cho Authentication
    /// Chỉ chứa các thao tác database, không có business logic
    /// </summary>
    internal class AuthDAL
    {
        private DatabaseContext database;

        public AuthDAL()
        {
            database = new DatabaseContext();
        }

        /// <summary>
        /// Lấy thông tin user và password từ database để xác thực
        /// </summary>
        public async Task<(User? user, string? passwordHash)> GetUserForLoginAsync(string username)
        {
            try
            {
                return await database.ExecuteWithConnectionAsync(async connection =>
                {
                    string query = @"
                        SELECT user_id, username, full_name, email, is_admin, is_active, password
                        FROM Users
                        WHERE username = @username AND is_active = TRUE";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username.Trim());

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int passwordIndex = reader.GetOrdinal("password");
                                int userIdIndex = reader.GetOrdinal("user_id");
                                int usernameIndex = reader.GetOrdinal("username");
                                int fullNameIndex = reader.GetOrdinal("full_name");
                                int emailIndex = reader.GetOrdinal("email");
                                int isAdminIndex = reader.GetOrdinal("is_admin");
                                int isActiveIndex = reader.GetOrdinal("is_active");
                                
                                string storedPassword = reader.GetString(passwordIndex);
                                
                                User user = new User
                                {
                                    UserId = reader.GetInt32(userIdIndex),
                                    Username = reader.GetString(usernameIndex),
                                    FullName = reader.GetString(fullNameIndex),
                                    Email = reader.IsDBNull(emailIndex) ? null : reader.GetString(emailIndex),
                                    IsAdmin = reader.GetInt32(isAdminIndex) != 0,
                                    IsActive = reader.GetInt32(isActiveIndex) != 0
                                };

                                return (user, storedPassword);
                            }
                        }
                    }
                    return (null, null);
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đăng nhập: {ex.Message}", ex);
            }
        }
    }
}

