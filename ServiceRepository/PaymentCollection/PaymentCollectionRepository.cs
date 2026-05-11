using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.CustomersRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using System.Data;

namespace HridhayConnect_API.ServiceRepository.PaymentCollectionRepository
{
    public class PaymentCollectionRepository : IPaymentCollectionRepository
    {
        private readonly DataContext _context;
        private readonly IRepositoryBase<PaymentCollection> _repositoryBase;

        public PaymentCollectionRepository(DataContext context, IRepositoryBase<PaymentCollection> repositoryBase)
        {
            _context = context;
            _repositoryBase = repositoryBase;
        }

        public async Task<PagedResult<PaymentCollection>> GetAllPaymentCollections(
            int start, int length, string sortColumn, string sortColumnDir, string searchValue)
        {
            try
            {
                SqlParameter[] parameters = null;

                var pagedResult = _repositoryBase.ExecuteWithPagination(
                    "sp_PaymentCollection_Get",
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

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdatePaymentCollection(PaymentCollection paymentCollection)
        {
            try
            {


                List<SqlParameter> parameters = new()
                {
                     new SqlParameter("Id", paymentCollection.Id),
                    new SqlParameter("CustomerId", (object?)paymentCollection.CustomerId ?? DBNull.Value),
                    new SqlParameter("Collected_By", (object?)paymentCollection.Collected_By ?? DBNull.Value),
                    new SqlParameter("PaymentDate", (object?)paymentCollection.PaymentDate ?? DBNull.Value),
                    new SqlParameter("PaymentMode", (object?)paymentCollection.PaymentMode ?? DBNull.Value),                   
                    new SqlParameter("ReferenceNo", (object?)paymentCollection.ReferenceNo ?? DBNull.Value),
                    new SqlParameter("Remarks", (object?)paymentCollection.Remarks ?? DBNull.Value),
                    new SqlParameter("Amount", (object?)paymentCollection.Amount ?? DBNull.Value),                  
                    new SqlParameter("Operated_By", AppHttpContextAccessor.JwtUserId),
                    new SqlParameter("Action", paymentCollection.Id == 0 ? "INSERT" : "UPDATE")
                };

              var result = _repositoryBase.ExecuteStoredProcedurenew("SP_PaymentCollection_Save", parameters,true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PaymentCollection?> GetPaymentCollectionById(long id)
        {
            try
            {
                var parameters = new[]
                {
         new SqlParameter("@Id", id)
     };

                var result = _repositoryBase.ExecuteSingle(
                    "sp_PaymentCollection_Get",
                    parameters
                );

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<PagedResult<PaymentCollection>> GetAllOutstandingPayments(
           int start, int length, string sortColumn, string sortColumnDir, string searchValue)
        {
            try
            {
                SqlParameter[] parameters = null;

                var pagedResult = _repositoryBase.ExecuteWithPagination(
                    "sp_Outstanding_Get",
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
        //public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteByPaymentCollectionId(long id)
        //{
        //    try
        //    {

        //        List<SqlParameter> parameters = new()
        //        {
        //            new SqlParameter("Id", id),
        //            new SqlParameter("Operated_By", AppHttpContextAccessor.JwtUserId)
        //        };

        //        var result = _repositoryBase.ExecuteStoredProcedurenew("sp_PaymentCollection_Delete", parameters,true);

        //        return await Task.FromResult(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }

}
