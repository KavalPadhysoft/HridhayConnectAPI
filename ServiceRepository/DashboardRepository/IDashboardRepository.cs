using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using Microsoft.AspNetCore.Mvc;
using HridhayConnect_API.Infra;

namespace HridhayConnect_API.ServiceRepository.DashboardRepository
{
    public interface IDashboardRepository
    {
        Task<PagedResult<Dashboard>> GetDashboardDataForPendingOrders(
            int start, int length, string sortColumn, string sortColumnDir, string searchValue);

        Task<PagedResult<PendingItem>> GetDashboardDataForPendingItemWise(
            int start, int length, string sortColumn, string sortColumnDir, string searchValue);


    }
}
