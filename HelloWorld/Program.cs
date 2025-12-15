namespace helloworld
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            
            // Hiển thị form đăng nhập đầu tiên
            using (DangNhapViews loginForm = new DangNhapViews())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Nếu đăng nhập thành công, mở form Main
                    Application.Run(new Main());
                }
            }
        }
    }
}