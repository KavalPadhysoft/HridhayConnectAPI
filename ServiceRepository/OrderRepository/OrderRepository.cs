using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.ItemRepository;
using System.Data;

namespace HridhayConnect_API.ServiceRepository.OrderRepository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _context;
        private readonly IRepositoryBase<Orders> _repositoryBase;

        public OrderRepository(DataContext context, IRepositoryBase<Orders> repositoryBase)
        {
            _context = context;
            _repositoryBase = repositoryBase;
        }

        public async Task<PagedResult<Orders>> GetAllOrders(
            int start, int length, string sortColumn, string sortColumnDir, string searchValue)
        {
            try
            {
                SqlParameter[] parameters = null;

                var pagedResult = _repositoryBase.ExecuteWithPagination(
                    "sp_Order_Get",
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

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdateOrder(OrderCombo orders)
        {
            try
            {
                var itemsJson = JsonConvert.SerializeObject(orders.OrderItems);

                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("@Id", orders.Order.Id),
                    new SqlParameter("@CustomerId", (object?)orders.Order.CustomerId ?? DBNull.Value),
                    new SqlParameter("@SalesPerson_Id", (object?)orders.Order.SalesPerson_Id ?? DBNull.Value),
                    new SqlParameter("@Order_No", (object?)orders.Order.Order_No ?? DBNull.Value),
                    new SqlParameter("@OrderDate", (object?)orders.Order.Order_Date ?? DBNull.Value),
                    new SqlParameter("@TotalAmount", (object?)orders.Order.Total_Amount ?? DBNull.Value),
                    new SqlParameter("@Order_Status", (object?)orders.Order.Order_Status ?? DBNull.Value),
                    new SqlParameter("@Notes", (object?)orders.Order .Notes ?? DBNull.Value),
                    new SqlParameter("@Items", itemsJson),  // 🔥 JSON
                    new SqlParameter("@Operated_By", AppHttpContextAccessor.JwtUserId),
                    new SqlParameter("@Action", orders.Order.Id == 0 ? "INSERT" : "UPDATE"),
                };

              var result = _repositoryBase.ExecuteStoredProcedurenew("sp_Order_Save", parameters,true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<OrderCombo?> GetOrderById(long id)
        {
            List<SqlParameter> parameters = new()
    {
        new SqlParameter("@Id", id)
    };

            var ds = _repositoryBase.ExecuteStoredProcedureDataSet("sp_Order_Get", parameters);

            if (ds == null || ds.Tables.Count < 2)
                return null;

            // 🔹 Order (first table)
            var orderJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0]);
            var order = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Orders>>(orderJson)?.FirstOrDefault();

            // 🔹 Items (second table)
            var itemsJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[1]);
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OrderItem>>(itemsJson);
            if (order == null) return null;

            return new OrderCombo
            {
                Order = order,
                OrderItems = items ?? new List<OrderItem>()
            };
        }
        public async Task<Orders?> GetOrderNo()
        {
            try
            {
                var parameters = new[]
                {
         new SqlParameter("@Id", -1)
        };

                var result = _repositoryBase.ExecuteSingle(
                    "sp_Order_Get",
                    parameters
                );

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteByOrderId(long id)
        {
            try
            {

                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("Id", id),
                    new SqlParameter("Operated_By", AppHttpContextAccessor.JwtUserId)
                };

                var result = _repositoryBase.ExecuteStoredProcedurenew("sp_Order_Delete", parameters,true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<OrderFullDTO?> GetOrderLayoutdata(long id)
        {
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@Id", id)                
            };

            var ds = _repositoryBase.ExecuteStoredProcedureDataSet("sp_OrderLayout_Get", parameters);

            if (ds == null || ds.Tables.Count < 2)
                return null;

            // 🔹 Order (first table)
            var orderJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0]);
            var order = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Orders>>(orderJson)?.FirstOrDefault();

            // 🔹 Items (second table)
            var itemsJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[1]);
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OrderItem>>(itemsJson);
            
            if (order == null) return null;

            return new OrderFullDTO
            {
                Order = order,
                OrderItems = items ?? new List<OrderItem>()
            };
        }

        public async Task<OrderFullDTO?> GetDeliveryLayoutdata(long id)
        {
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@Id", id)
            };

            var ds = _repositoryBase.ExecuteStoredProcedureDataSet("sp_DeliveryLayout_Get", parameters);

            if (ds == null || ds.Tables.Count < 2)
                return null;

            // 🔹 Order (first table)
            var orderJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0]);
            var order = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Orders>>(orderJson)?.FirstOrDefault();

            // 🔹 Items (second table)
            var itemsJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[1]);
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OrderItem>>(itemsJson);

            if (order == null) return null;

            return new OrderFullDTO
            {
                Order = order,
                OrderItems = items ?? new List<OrderItem>()
            };
        }
    }

}
