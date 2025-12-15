using System;

namespace helloworld
{
    /// <summary>
    /// Class quản lý thông tin user đang đăng nhập (Session)
    /// </summary>
    public static class UserSession
    {
        private static User? currentUser = null;

        /// <summary>
        /// Lưu thông tin user đang đăng nhập
        /// </summary>
        /// <param name="user">Thông tin user</param>
        public static void SetCurrentUser(User user)
        {
            currentUser = user;
        }

        /// <summary>
        /// Lấy thông tin user đang đăng nhập
        /// </summary>
        /// <returns>User object hoặc null nếu chưa đăng nhập</returns>
        public static User? GetCurrentUser()
        {
            return currentUser;
        }

        /// <summary>
        /// Lấy User ID của user đang đăng nhập
        /// </summary>
        /// <returns>User ID hoặc 0 nếu chưa đăng nhập</returns>
        public static int GetCurrentUserId()
        {
            return currentUser?.UserId ?? 0;
        }

        /// <summary>
        /// Lấy tên đầy đủ của user đang đăng nhập
        /// </summary>
        /// <returns>Tên đầy đủ hoặc string.Empty nếu chưa đăng nhập</returns>
        public static string GetCurrentUserFullName()
        {
            return currentUser?.FullName ?? string.Empty;
        }

        /// <summary>
        /// Lấy username của user đang đăng nhập
        /// </summary>
        /// <returns>Username hoặc string.Empty nếu chưa đăng nhập</returns>
        public static string GetCurrentUsername()
        {
            return currentUser?.Username ?? string.Empty;
        }

        /// <summary>
        /// Kiểm tra user có phải admin không
        /// </summary>
        /// <returns>True nếu là admin</returns>
        public static bool IsAdmin()
        {
            return currentUser?.IsAdmin ?? false;
        }

        /// <summary>
        /// Đăng xuất - Xóa thông tin user
        /// </summary>
        public static void Logout()
        {
            currentUser = null;
        }

        /// <summary>
        /// Kiểm tra user đã đăng nhập chưa
        /// </summary>
        /// <returns>True nếu đã đăng nhập</returns>
        public static bool IsLoggedIn()
        {
            return currentUser != null;
        }
    }
}

