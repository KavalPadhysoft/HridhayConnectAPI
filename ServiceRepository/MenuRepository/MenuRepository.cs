using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.MenuRepository;
using System.Data;

namespace HridhayConnect_API.ServiceRepository.MenuRepository
{
    public class MenuRepository : IMenuRepository
    {
        private readonly DataContext _context;
        private readonly IRepositoryBase<Menu> _repositoryBase;

        public MenuRepository(DataContext context, IRepositoryBase<Menu> repositoryBase)
        {
            _context = context;
            _repositoryBase = repositoryBase;
        }

        public async Task<PagedResult<Menu>> GetAllMenu(
            int start, int length, string sortColumn, string sortColumnDir, string searchValue)
        {
            try
            {
                SqlParameter[] parameters = null;

                var pagedResult = _repositoryBase.ExecuteWithPagination(
                    "SP_Menu_Get",
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

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdateMenu(Menu menu)
        {
            try
            {


                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("Id", menu.Id),
                    new SqlParameter("ParentId", menu.ParentId),
                    new SqlParameter("Area", (object?)menu.Area ?? DBNull.Value),
                    new SqlParameter("Controller", (object?)menu.Controller ?? DBNull.Value),
                    new SqlParameter("Url", (object?)menu.Url ?? DBNull.Value),
                    new SqlParameter("Name", (object?)menu.Name ?? DBNull.Value),
                    new SqlParameter("Icon", (object?)menu.Icon ?? DBNull.Value),
                    new SqlParameter("DisplayOrder", (object?)menu.DisplayOrder ?? DBNull.Value),
                    new SqlParameter("IsSuperAdmin", (object?)menu.IsSuperAdmin ?? DBNull.Value),
                    new SqlParameter("IsAdmin", (object?)menu.IsAdmin ?? DBNull.Value),
                    new SqlParameter("Operated_By", AppHttpContextAccessor.JwtUserId),
                    new SqlParameter("Action", menu.Id == 0 ? "INSERT" : "UPDATE")
                };

              var result = _repositoryBase.ExecuteStoredProcedurenew("SP_Menu_Save", parameters,true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Menu?> GetMenuById(long id)
        {
            try
            {
                var parameters = new[]
                {
         new SqlParameter("@Id", id)
     };

                var result = _repositoryBase.ExecuteSingle(
                    "SP_Menu_Get",
                    parameters
                );

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteByMenuId(long id)
        {
            try
            {

                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("Id", id),
                    new SqlParameter("Operated_By", AppHttpContextAccessor.JwtUserId)
                };

                var result = _repositoryBase.ExecuteStoredProcedurenew("sp_Menu_Delete", parameters,true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
