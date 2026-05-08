using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.ItemRepository;
using System.Data;

namespace HridhayConnect_API.ServiceRepository.DashboardRepository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DataContext _context;
        private readonly IRepositoryBase<Dashboard> _repositoryBase;
        private readonly IRepositoryBase<PendingItem> _pendingItemRepositoryBase;

        public DashboardRepository(DataContext context, IRepositoryBase<Dashboard> repositoryBase, IRepositoryBase<PendingItem> pendingItemRepositoryBase)
        {
            _context = context;
            _repositoryBase = repositoryBase;
            _pendingItemRepositoryBase = pendingItemRepositoryBase;
        }

        public async Task<PagedResult<Dashboard>> GetDashboardDataForPendingOrders(
           int start, int length, string sortColumn, string sortColumnDir, string searchValue)
        {
            try
            {
                SqlParameter[] parameters = null;

                var pagedResult = _repositoryBase.ExecuteWithPagination(
                    "sp_PendingOrder_ForDashboard_Get",
                    parameters,
                    start,
                    length,
                    sortColumn,
                    sortColumnDir,
                    searchValue
                );

                return await Task.FromResult(pagedResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<PagedResult<PendingItem>> GetDashboardDataForPendingItemWise(
           int start, int length, string sortColumn, string sortColumnDir, string searchValue)
        {
            try
            {
                SqlParameter[] parameters = null;

                var pagedResult = _pendingItemRepositoryBase.ExecuteWithPagination(
                    "sp_PendingItem_Wise_ForDashboard_Get",
                    parameters,
                    start,
                    length,
                    sortColumn,
                    sortColumnDir,
                    searchValue
                );

                return await Task.FromResult(pagedResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
