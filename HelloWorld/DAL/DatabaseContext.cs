using System;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace helloworld.DAL
{
    /// <summary>
    /// Class quản lý kết nối tới MySQL database (XAMPP)
    /// </summary>
    public class DatabaseContext : IDisposable
    {
        private MySqlConnection? connection;
        private readonly string connectionString;

        // Thông tin kết nối mặc định cho XAMPP MySQL
        private const string DEFAULT_SERVER = "localhost";
        private const string DEFAULT_PORT = "3306";
        private const string DEFAULT_USERNAME = "root";
        private const string DEFAULT_PASSWORD = "";
        private const string DEFAULT_DATABASE = "crm_store";

        public DatabaseContext()
        {
            connectionString = BuildConnectionString(
                DEFAULT_SERVER,
                DEFAULT_PORT,
                DEFAULT_DATABASE,
                DEFAULT_USERNAME,
                DEFAULT_PASSWORD
            );
        }

        public DatabaseContext(string server, string port, string database, string username, string password)
        {
            connectionString = BuildConnectionString(server, port, database, username, password);
        }

        private string BuildConnectionString(string server, string port, string database, string username, string password)
        {
            return $"Server={server};Port={port};Database={database};Uid={username};Pwd={password};CharSet=utf8mb4;";
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public async Task<T> ExecuteWithConnectionAsync<T>(Func<MySqlConnection, Task<T>> action)
        {
            using (var conn = CreateConnection())
            {
                await conn.OpenAsync();
                return await action(conn);
            }
        }

        public async Task ExecuteWithConnectionAsync(Func<MySqlConnection, Task> action)
        {
            using (var conn = CreateConnection())
            {
                await conn.OpenAsync();
                await action(conn);
            }
        }

        public async Task<MySqlConnection> OpenConnectionAsync()
        {
            if (connection == null || connection.State != ConnectionState.Open)
            {
                connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();
            }
            return connection;
        }

        public void CloseConnection()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                connection.Dispose();
                connection = null;
            }
        }

        public void Dispose()
        {
            CloseConnection();
        }
    }
}
