using System;
using System.Data;
using System.Threading.Tasks;

namespace helloworld
{
    /// <summary>
    /// Business Logic Layer cho Categories
    /// Chứa business logic, validation, và gọi CategoryDAL
    /// Giữ nguyên interface public như DanhMucModels cũ để tương thích với Forms
    /// </summary>
    internal class CategoryBLL
    {
        private CategoryDAL categoryDAL;

        public CategoryBLL()
        {
            categoryDAL = new CategoryDAL();
        }

        /// <summary>
        /// Lấy danh sách danh mục từ database
        /// </summary>
        public async Task<DataTable> LoadCategoriesAsync()
        {
            return await categoryDAL.LoadCategoriesAsync();
        }

        /// <summary>
        /// Cập nhật thông tin danh mục
        /// </summary>
        public async Task UpdateCategoryAsync(int categoryId, string categoryName, string? description)
        {
            // Validate
            if (categoryId <= 0)
            {
                throw new ArgumentException("Category ID không hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("Tên danh mục không được để trống.");
            }

            await categoryDAL.UpdateCategoryAsync(categoryId, categoryName, description);
        }

        /// <summary>
        /// Thêm danh mục mới vào database
        /// </summary>
        public async Task<int> AddCategoryAsync(string categoryName, string? description)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("Tên danh mục không được để trống.");
            }

            return await categoryDAL.AddCategoryAsync(categoryName, description);
        }

        /// <summary>
        /// Xóa danh mục khỏi database
        /// </summary>
        public async Task DeleteCategoryAsync(int categoryId)
        {
            await categoryDAL.DeleteCategoryAsync(categoryId);
        }
    }
}

