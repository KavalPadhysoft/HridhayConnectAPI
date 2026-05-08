using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.ItemRepository;
using System.Data;

namespace HridhayConnect_API.ServiceRepository.DeliveriesRepository
{
    public class DeliveriesRepository : IDeliveriesRepository
    {
        private readonly DataContext _context;
        private readonly IRepositoryBase<Deliveries> _repositoryBase;

        public DeliveriesRepository(DataContext context, IRepositoryBase<Deliveries> repositoryBase)
        {
            _context = context;
            _repositoryBase = repositoryBase;
        }

       
        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdateDelivery(DeliveriesCombo deliveries)
        {
            try
            {
                var itemsJson = JsonConvert.SerializeObject(deliveries.DeliveryItems);
                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("@Order_ID", (object?)deliveries.Order.Id ?? DBNull.Value),
                    new SqlParameter("@Delivery_Date", (object?)deliveries.Order.Delivery_Date ?? DBNull.Value),
                    new SqlParameter("@TotalAmount", (object?)deliveries.Order.Total_Amount ?? DBNull.Value),
                    new SqlParameter("@Remarks", (object?)deliveries.Order .Remarks ?? DBNull.Value),
                    new SqlParameter("@Items", itemsJson),  // 🔥 JSON
                    new SqlParameter("@Operated_By", AppHttpContextAccessor.JwtUserId),
                    
                };

              var result = _repositoryBase.ExecuteStoredProcedurenew("sp_Deliveries_Save", parameters,true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DeliveriesCombo?> GetDeliveriesById(long id)
        {
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@Id", id)
            };

            var ds = _repositoryBase.ExecuteStoredProcedureDataSet("sp_Deliveries_Get", parameters);

            if (ds == null || ds.Tables.Count < 2)
                return null;

            // 🔹 Order (first table)
            var orderJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0]);
            var order = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Deliveries>>(orderJson)?.FirstOrDefault();

            // 🔹 Items (second table)
            var itemsJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[1]);
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DeliveriesDetails>>(itemsJson);
            if (order == null) return null;

            return new DeliveriesCombo
            {
                Order = order,
                DeliveryItems = items ?? new List<DeliveriesDetails>()
            };
        }
       

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> CancelOrder(long id)
        {
            try
            {

                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("Id", id),
                    new SqlParameter("Operated_By", AppHttpContextAccessor.JwtUserId)
                };

                var result = _repositoryBase.ExecuteStoredProcedurenew("sp_Cancel_Order", parameters,true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
