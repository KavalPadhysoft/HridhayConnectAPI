using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using Microsoft.AspNetCore.Mvc;
using HridhayConnect_API.Infra;

namespace HridhayConnect_API.ServiceRepository.CategoryRepository
{
    public interface ICategoryRepository
    {
        Task<PagedResult<Category>> GetAllCategory(
            int start, int length, string sortColumn, string sortColumnDir, string searchValue);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdateCategory(Category category);

        Task<Category?> GetCategoryById(long id);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteByCategoryId(long id);
    }
}
