using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.ItemRepository;
using System.Data;

namespace HridhayConnect_API.ServiceRepository.ItemRepository
{
    public class ItemRepository : IItemRepository
    {
        private readonly DataContext _context;
        private readonly IRepositoryBase<Item> _repositoryBase;

        public ItemRepository(DataContext context, IRepositoryBase<Item> repositoryBase)
        {
            _context = context;
            _repositoryBase = repositoryBase;
        }

        public async Task<PagedResult<Item>> GetAllItem(
            int start, int length, string sortColumn, string sortColumnDir, string searchValue)
        {
            try
            {
                SqlParameter[] parameters = null;

                var pagedResult = _repositoryBase.ExecuteWithPagination(
                    "sp_Item_Get",
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

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdateItem(Item item  )
        {
            try
            {


                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("@Id", item.Id),
                    new SqlParameter("@CategoryId", (object?)item.CategoryId ?? DBNull.Value),
                    new SqlParameter("@ItemName", (object?)item.ItemName ?? DBNull.Value),
                    new SqlParameter("@PackagingType", (object?)item.PackagingType ?? DBNull.Value),
                    new SqlParameter("@Unit", (object?)item.Unit ?? DBNull.Value),
                    new SqlParameter("@TaxPercent", (object?)item.TaxPercent ?? DBNull.Value),
                    new SqlParameter("@Description", (object?)item.Description ?? DBNull.Value),
                    new SqlParameter("@Rate", (object?)item.Rate ?? DBNull.Value),
                    new SqlParameter("@MRP", (object?)item.MRP ?? DBNull.Value),
                    new SqlParameter("@IsAvailableForSale", (object?)item.IsAvailableForSale ?? DBNull.Value),
                    new SqlParameter("@IsActive", (object?)item.IsActive ?? DBNull.Value),
                    new SqlParameter("@Operated_By", AppHttpContextAccessor.JwtUserId),
                    new SqlParameter("@Action", item.Id == 0 ? "INSERT" : "UPDATE"),
                };

              var result = _repositoryBase.ExecuteStoredProcedurenew("SP_Item_Save", parameters,true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Item?> GetItemById(long id)
        {
            try
            {
                var parameters = new[]
                {
         new SqlParameter("@Id", id)
     };

                var result = _repositoryBase.ExecuteSingle(
                    "sp_Item_Get",
                    parameters
                );

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteByItemId(long id)
        {
            try
            {

                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("Id", id),
                    new SqlParameter("Operated_By", AppHttpContextAccessor.JwtUserId)
                };

                var result = _repositoryBase.ExecuteStoredProcedurenew("sp_Item_Delete", parameters,true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
