using System;

namespace helloworld
{
    /// <summary>
    /// Class quản lý thông tin user đang đăng nhập (Session)
    /// </summary>
    public static class UserSession
    {
        private static User? currentUser = null;
        public static HashSet<string> CurrentPermissions { get; private set; } = new HashSet<string>();

        /// <summary>
        /// Lưu thông tin user đang đăng nhập và thiết lập quyền
        /// </summary>
        /// <param name="user">Thông tin user</param>
        public static void SetCurrentUser(User user)
        {
            currentUser = user;
            CurrentPermissions.Clear();
            
            // Thiết lập quyền dựa trên IsAdmin
            // Chủ cửa hàng (Admin)
            if (user.IsAdmin)
            {
                // Full quyền
                CurrentPermissions.Add(Common.PermissionConstants.Product_View);
                CurrentPermissions.Add(Common.PermissionConstants.Product_Add);
                CurrentPermissions.Add(Common.PermissionConstants.Product_Edit);
                CurrentPermissions.Add(Common.PermissionConstants.Product_Delete);

                CurrentPermissions.Add(Common.PermissionConstants.Order_Create);
                CurrentPermissions.Add(Common.PermissionConstants.Order_View_All);
                CurrentPermissions.Add(Common.PermissionConstants.Order_Edit_Own);
                CurrentPermissions.Add(Common.PermissionConstants.Order_Edit_Others);

                CurrentPermissions.Add(Common.PermissionConstants.Report_View);
                CurrentPermissions.Add(Common.PermissionConstants.User_Manage);
            }
            // Nhân viên bán hàng
            else
            {
                // Quyền hạn chế
                CurrentPermissions.Add(Common.PermissionConstants.Product_View);
                CurrentPermissions.Add(Common.PermissionConstants.Product_Add);
                CurrentPermissions.Add(Common.PermissionConstants.Product_Edit);
                // KHÔNG CÓ quyền xóa sản phẩm (Product_Delete)

                CurrentPermissions.Add(Common.PermissionConstants.Order_Create);
                CurrentPermissions.Add(Common.PermissionConstants.Order_View_All); // Tạm thời cho xem hết, nhưng không được sửa của người khác
                CurrentPermissions.Add(Common.PermissionConstants.Order_Edit_Own);
                // KHÔNG CÓ quyền sửa đơn người khác (Order_Edit_Others)

                // KHÔNG CÓ quyền xem báo cáo (Report_View)
                // KHÔNG CÓ quyền quản lý nhân viên (User_Manage)
            }
        }

        /// <summary>
        /// Kiểm tra user có quyền cụ thể không
        /// </summary>
        public static bool HasPermission(string permissionCode)
        {
            if (currentUser == null) return false;
            return CurrentPermissions.Contains(permissionCode);
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
            CurrentPermissions.Clear();
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

