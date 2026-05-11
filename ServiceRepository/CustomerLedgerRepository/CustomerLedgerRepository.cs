using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.CustomerLedgerRepository;
using System.Data;

namespace HridhayConnect_API.ServiceRepository.CustomerLedgerRepository
{
    public class CustomerLedgerRepository : ICustomerLedgerRepository
    {
        private readonly DataContext _context;
        private readonly IRepositoryBase<CustomerLedger> _repositoryBase;

        public CustomerLedgerRepository(DataContext context, IRepositoryBase<CustomerLedger> repositoryBase)
        {
            _context = context;
            _repositoryBase = repositoryBase;
        }

        public async Task<PagedResult<CustomerLedger>> CustomerLedger(
            int start, int length, string sortColumn, string sortColumnDir, string searchValue, long? customerId)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                        new SqlParameter("@CustomerId", (object?)customerId ?? DBNull.Value)
                };

                var pagedResult = _repositoryBase.ExecuteWithPagination(
                    "sp_Customer_Ledger_Get",
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
