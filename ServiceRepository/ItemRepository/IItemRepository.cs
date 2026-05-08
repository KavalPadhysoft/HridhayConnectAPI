using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using Microsoft.AspNetCore.Mvc;
using HridhayConnect_API.Infra;

namespace HridhayConnect_API.ServiceRepository.ItemRepository
{
    public interface IItemRepository
    {
        Task<PagedResult<Item>> GetAllItem(
            int start, int length, string sortColumn, string sortColumnDir, string searchValue);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdateItem(Item item);

        Task<Item?> GetItemById(long id);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteByItemId(long id);
    }
}
