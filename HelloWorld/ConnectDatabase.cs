using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace helloworld
{
    /// <summary>
    /// Class quản lý kết nối tới MySQL database (XAMPP)
    /// </summary>
    internal class ConnectDatabase
    {
        private MySqlConnection? connection;
        private readonly string connectionString;

        // Thông tin kết nối mặc định cho XAMPP MySQL
        private const string DEFAULT_SERVER = "localhost";
        private const string DEFAULT_PORT = "3306";
        private const string DEFAULT_USERNAME = "root";
        private const string DEFAULT_PASSWORD = "";
        private const string DEFAULT_DATABASE = "crm_store"; // Tên database của bạn

        /// <summary>
        /// Constructor - Khởi tạo connection string mặc định
        /// </summary>
        public ConnectDatabase()
        {
            connectionString = BuildConnectionString(
                DEFAULT_SERVER,
                DEFAULT_PORT,
                DEFAULT_DATABASE,
                DEFAULT_USERNAME,
                DEFAULT_PASSWORD
            );
        }

        /// <summary>
        /// Constructor với thông tin kết nối tùy chỉnh
        /// </summary>
        public ConnectDatabase(string server, string port, string database, string username, string password)
        {
            connectionString = BuildConnectionString(server, port, database, username, password);
        }

        /// <summary>
        /// Xây dựng connection string
        /// </summary>
        private string BuildConnectionString(string server, string port, string database, string username, string password)
        {
            return $"Server={server};Port={port};Database={database};Uid={username};Pwd={password};CharSet=utf8mb4;";
        }

        /// <summary>
        /// Lấy connection string hiện tại
        /// </summary>
        public string GetConnectionString()
        {
            return connectionString;
        }

        /// <summary>
        /// Tạo connection mới (khuyến nghị sử dụng cho async operations để tránh conflict)
        /// </summary>
        /// <returns>MySqlConnection mới (chưa mở)</returns>
        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Thực thi action với connection riêng (tự động mở và đóng connection)
        /// Giúp code ngắn gọn hơn, tránh lặp lại pattern using/OpenAsync
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
        /// <param name="action">Action cần thực thi với connection đã mở</param>
        /// <returns>Kết quả từ action</returns>
        public async Task<T> ExecuteWithConnectionAsync<T>(Func<MySqlConnection, Task<T>> action)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                return await action(connection);
            }
        }

        /// <summary>
        /// Thực thi action với connection riêng (không trả về giá trị)
        /// </summary>
        /// <param name="action">Action cần thực thi với connection đã mở</param>
        public async Task ExecuteWithConnectionAsync(Func<MySqlConnection, Task> action)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                await action(connection);
            }
        }

        /// <summary>
        /// Mở kết nối tới database (Async)
        /// </summary>
        /// <returns>MySqlConnection đã mở</returns>
        public async Task<MySqlConnection> OpenConnectionAsync()
        {
            try
            {
                if (connection == null || connection.State != ConnectionState.Open)
                {
                    connection = new MySqlConnection(connectionString);
                    await connection.OpenAsync();
                }
                return connection;
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Lỗi kết nối database: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra kết nối database có hoạt động không (Async)
        /// </summary>
        /// <returns>True nếu kết nối thành công, False nếu không</returns>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (var testConnection = new MySqlConnection(connectionString))
                {
                    await testConnection.OpenAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy connection hiện tại (nếu đã mở)
        /// </summary>
        public MySqlConnection? GetConnection()
        {
            return connection?.State == ConnectionState.Open ? connection : null;
        }

        /// <summary>
        /// Đóng kết nối database
        /// </summary>
        public void CloseConnection()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                connection.Dispose();
                connection = null;
            }
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            CloseConnection();
        }
    }
}
