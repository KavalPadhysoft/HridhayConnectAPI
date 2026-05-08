using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.CategoryRepository;
using System.Data;

namespace HridhayConnect_API.ServiceRepository.CategoryRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;
        private readonly IRepositoryBase<Category> _repositoryBase;

        public CategoryRepository(DataContext context, IRepositoryBase<Category> repositoryBase)
        {
            _context = context;
            _repositoryBase = repositoryBase;
        }

        public async Task<PagedResult<Category>> GetAllCategory(
            int start, int length, string sortColumn, string sortColumnDir, string searchValue)
        {
            try
            {
                SqlParameter[] parameters = null;

                var pagedResult = _repositoryBase.ExecuteWithPagination(
                    "sp_Category_Get",
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

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdateCategory(Category category)
        {
            try
            {


                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("Id", category.Id),
                    new SqlParameter("Name", (object?)category.Name ?? DBNull.Value),
                    new SqlParameter("Operated_By", AppHttpContextAccessor.JwtUserId),
                    new SqlParameter("Action", category.Id == 0 ? "INSERT" : "UPDATE")
                };

              var result = _repositoryBase.ExecuteStoredProcedurenew("SP_Category_Save", parameters,true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Category?> GetCategoryById(long id)
        {
            try
            {
                var parameters = new[]
                {
         new SqlParameter("@Id", id)
     };

                var result = _repositoryBase.ExecuteSingle(
                    "sp_Category_Get",
                    parameters
                );

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteByCategoryId(long id)
        {
            try
            {

                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("Id", id),
                    new SqlParameter("Operated_By", AppHttpContextAccessor.JwtUserId)
                };

                var result = _repositoryBase.ExecuteStoredProcedurenew("sp_Category_Delete", parameters,true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
