using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using Microsoft.AspNetCore.Mvc;
using HridhayConnect_API.Infra;

namespace HridhayConnect_API.ServiceRepository.MenuRepository
{
    public interface IMenuRepository
    {
        Task<PagedResult<Menu>> GetAllMenu(
            int start, int length, string sortColumn, string sortColumnDir, string searchValue);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdateMenu(Menu menu);

        Task<Menu?> GetMenuById(long id);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteByMenuId(long id);
    }
}
