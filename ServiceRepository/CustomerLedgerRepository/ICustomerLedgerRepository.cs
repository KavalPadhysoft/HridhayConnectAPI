using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using Microsoft.AspNetCore.Mvc;
using HridhayConnect_API.Infra;

namespace HridhayConnect_API.ServiceRepository.CustomerLedgerRepository
{
    public interface ICustomerLedgerRepository
    {
        Task<PagedResult<CustomerLedger>> CustomerLedger(int start, int length, string sortColumn, string sortColumnDir, string searchValue,
               
     long? customerId
    );

       
    }
}
