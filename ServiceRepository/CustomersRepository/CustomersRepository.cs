using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.CustomersRepository;
using System.Data;

namespace HridhayConnect_API.ServiceRepository.CustomersRepository
{
    public class CustomersRepository : ICustomersRepository
    {
        private readonly DataContext _context;
        private readonly IRepositoryBase<Customers> _repositoryBase;

        public CustomersRepository(DataContext context, IRepositoryBase<Customers> repositoryBase)
        {
            _context = context;
            _repositoryBase = repositoryBase;
        }

        public async Task<PagedResult<Customers>> GetAllCustomers(
            int start, int length, string sortColumn, string sortColumnDir, string searchValue)
        {
            try
            {
                SqlParameter[] parameters = null;

                var pagedResult = _repositoryBase.ExecuteWithPagination(
                    "sp_Customer_Get",
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

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdateCustomers(Customers customers)
        {
            try
            {


                List<SqlParameter> parameters = new()
                {
                     new SqlParameter("Id", customers.Id),

                    new SqlParameter("Name", (object?)customers.Name ?? DBNull.Value),
                    new SqlParameter("OwnerName", (object?)customers.OwnerName ?? DBNull.Value),
                    new SqlParameter("Phone", (object?)customers.Phone ?? DBNull.Value),
                    new SqlParameter("Email", (object?)customers.Email ?? DBNull.Value),
                    new SqlParameter("GSTNumber", (object?)customers.GSTNumber ?? DBNull.Value),
                    new SqlParameter("CustomerType", (object?)customers.CustomerType ?? DBNull.Value),

                    new SqlParameter("AddressLine", (object?)customers.AddressLine ?? DBNull.Value),
                    new SqlParameter("Area", (object?)customers.Area ?? DBNull.Value),
                    new SqlParameter("City", (object?)customers.City ?? DBNull.Value),
                    new SqlParameter("State", (object?)customers.State ?? DBNull.Value),
                    new SqlParameter("Pincode", (object?)customers.Pincode ?? DBNull.Value),

                    new SqlParameter("CreditLimit", (object?)customers.CreditLimit ?? DBNull.Value),
                    new SqlParameter("CreditDays", (object?)customers.CreditDays ?? DBNull.Value),

                    new SqlParameter("IsActive", (object?)customers.IsActive ?? DBNull.Value),

                    new SqlParameter("Operated_By", AppHttpContextAccessor.JwtUserId),
                    new SqlParameter("Action", customers.Id == 0 ? "INSERT" : "UPDATE")
                };

              var result = _repositoryBase.ExecuteStoredProcedurenew("SP_Customer_Save", parameters,true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Customers?> GetCustomerById(long id)
        {
            try
            {
                var parameters = new[]
                {
         new SqlParameter("@Id", id)
     };

                var result = _repositoryBase.ExecuteSingle(
                    "sp_Customer_Get",
                    parameters
                );

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteByCustomerId(long id)
        {
            try
            {

                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("Id", id),
                    new SqlParameter("Operated_By", AppHttpContextAccessor.JwtUserId)
                };

                var result = _repositoryBase.ExecuteStoredProcedurenew("sp_Customer_Delete", parameters,true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
