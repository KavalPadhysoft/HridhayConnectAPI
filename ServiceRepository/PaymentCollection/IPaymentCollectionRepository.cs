using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using Microsoft.AspNetCore.Mvc;
using HridhayConnect_API.Infra;

namespace HridhayConnect_API.ServiceRepository.PaymentCollectionRepository
{
    public interface IPaymentCollectionRepository
    {
        Task<PagedResult<PaymentCollection>> GetAllPaymentCollections(
            int start, int length, string sortColumn, string sortColumnDir, string searchValue);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdatePaymentCollection(PaymentCollection paymentCollection);

        Task<PaymentCollection?> GetPaymentCollectionById(long id);

        Task<PagedResult<PaymentCollection>> GetAllOutstandingPayments(
            int start, int length, string sortColumn, string sortColumnDir, string searchValue);

        //Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteByPaymentCollectionId(long id);
    }
}
