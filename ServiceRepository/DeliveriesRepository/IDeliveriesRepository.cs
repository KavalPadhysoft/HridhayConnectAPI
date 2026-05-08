using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using Microsoft.AspNetCore.Mvc;
using HridhayConnect_API.Infra;

namespace HridhayConnect_API.ServiceRepository.DeliveriesRepository
{
    public interface IDeliveriesRepository
    {
      

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdateDelivery(DeliveriesCombo deliveries);
        Task<DeliveriesCombo?> GetDeliveriesById(long id);
        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> CancelOrder(long id);
    }
}
