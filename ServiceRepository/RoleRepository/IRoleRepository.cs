
using HridhayConnect_API.Models;
using Microsoft.AspNetCore.Mvc;
using HridhayConnect_API.Infra;

namespace HridhayConnect_API.ServiceRepository.RoleRepository
{
    public interface IRoleRepository
    {
        Task<PagedResult<Role>> GetAllRoles(int start, int length, string sortColumn, string sortColumnDir, string searchValue);
        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddUpdateRole(Role role);
        Task<Role> GetRoleById(long id);
        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteRole(long id);

    }
}
