namespace helloworld.Common
{
    /// <summary>
    /// Định nghĩa các quyền hạn trong hệ thống
    /// </summary>
    public static class PermissionConstants
    {
        // Quyền quản lý sản phẩm
        public const string Product_View = "Product_View";
        public const string Product_Add = "Product_Add";
        public const string Product_Edit = "Product_Edit";
        public const string Product_Delete = "Product_Delete";

        // Quyền quản lý đơn hàng
        public const string Order_Create = "Order_Create";
        public const string Order_View_All = "Order_View_All"; // Xem tất cả đơn hàng
        public const string Order_Edit_Own = "Order_Edit_Own"; // Sửa đơn hàng của chính mình
        public const string Order_Edit_Others = "Order_Edit_Others"; // Sửa đơn hàng của người khác

        // Quyền xem báo cáo
        public const string Report_View = "Report_View";

        // Quyền quản lý nhân viên
        public const string User_Manage = "User_Manage";
    }
}
