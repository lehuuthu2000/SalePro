namespace helloworld
{
    /// <summary>
    /// Entity class đại diện cho người dùng
    /// </summary>
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
    }
}

