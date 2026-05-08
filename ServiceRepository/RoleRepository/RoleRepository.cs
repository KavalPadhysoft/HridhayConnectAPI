using HridhayConnect_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using HridhayConnect_API.Infra;

using System.Data;

namespace HridhayConnect_API.ServiceRepository.RoleRepository
{
    public class RoleRepository : IRoleRepository
    {

        private readonly DataContext _context;
        private readonly IRepositoryBase<Role> _repositoryBase;

        public RoleRepository(DataContext context, IRepositoryBase<Role> repositoryBase)
        {
            _context = context;
            _repositoryBase = repositoryBase;
        }
        public async Task<PagedResult<Role>> GetAllRoles(int start, int length, string sortColumn, string sortColumnDir, string searchValue)
        {
            try
            {
                var parameters = new[]
                {
               new SqlParameter("@role_id", SqlDbType.Int) { Value = 0 }
            };



                var pagedResult = _repositoryBase.ExecuteWithPagination(
                    "sp_Role_Get",
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
        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddUpdateRole(Role role)
        {
            try
            {


                List<SqlParameter> oParams = new();

                
                    oParams.Add(new SqlParameter("@RoleId", role.Id));
                    oParams.Add(new SqlParameter("@RoleName", role.Name ?? (object)DBNull.Value));
                    oParams.Add(new SqlParameter("@IsAdmin", role.IsAdmin ?? (object)DBNull.Value));
                    oParams.Add(new SqlParameter("@SelectedMenu",  role.SelectedMenu ?? (object)DBNull.Value));
                   oParams.Add(new SqlParameter("@Operated_By",AppHttpContextAccessor.JwtUserId));
                    oParams.Add(new SqlParameter("@Action", role.Id == 0 ? "INSERT" : "UPDATE"));


                var result =_repositoryBase.ExecuteStoredProcedurenew("sp_Role_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                //var response = new CommonViewModel
                //{
                //    IsSuccess = false,
                //    StatusCode = ResponseStatusCode.Error,
                //    Message = ex.Message
                //};

                //return new JsonResult(response);
                throw ex;
            }
        }

        public async Task<Role> GetRoleById(long id)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@role_id", id)
                };

                var result = _repositoryBase.ExecuteSingle(
                    "sp_Role_Get",
                    parameters
                );

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
      
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteRole(long id)
        {
            try
            {
                List<SqlParameter> parameters = new()
              
                {
                 new SqlParameter("RoleId", id),
                 new SqlParameter("Operated_By", AppHttpContextAccessor.JwtUserId)
                };

                var result = _repositoryBase.ExecuteStoredProcedurenew("sp_Role_Delete", parameters,true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
