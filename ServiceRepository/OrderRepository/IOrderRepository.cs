using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using Microsoft.AspNetCore.Mvc;
using HridhayConnect_API.Infra;

namespace HridhayConnect_API.ServiceRepository.OrderRepository
{
    public interface IOrderRepository
    {
        Task<PagedResult<Orders>> GetAllOrders(
            int start, int length, string sortColumn, string sortColumnDir, string searchValue);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdateOrder(OrderCombo orders);
        Task<OrderCombo?> GetOrderById(long id);
        Task<Orders?> GetOrderNo();
        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteByOrderId(long id);

        Task<OrderFullDTO?> GetOrderLayoutdata(long id);
        Task<OrderFullDTO?> GetDeliveryLayoutdata(long id);
    }
}
