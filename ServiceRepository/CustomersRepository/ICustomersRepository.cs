using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using Microsoft.AspNetCore.Mvc;
using HridhayConnect_API.Infra;

namespace HridhayConnect_API.ServiceRepository.CustomersRepository
{
    public interface ICustomersRepository
    {
        Task<PagedResult<Customers>> GetAllCustomers(
            int start, int length, string sortColumn, string sortColumnDir, string searchValue);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdateCustomers(Customers customers);

        Task<Customers?> GetCustomerById(long id);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteByCustomerId(long id);
    }
}
