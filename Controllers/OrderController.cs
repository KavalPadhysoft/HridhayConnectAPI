using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.ItemRepository;
using HridhayConnect_API.ServiceRepository.MenuRepository;
using HridhayConnect_API.ServiceRepository.OrderRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace HridhayConnect_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly CommonViewModel CommonViewModel = new();
        private readonly ValidationService _validation;

        public OrderController(IOrderRepository orderRepository, ValidationService validation)
        {
            _orderRepository = orderRepository;
            _validation = validation;
        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> GetAllpage(
            int start = 0, int length = 10,
            string sortColumn = "", string sortColumnDir = "asc",
            string searchValue = "")
        {
            try
            {
                var data = await _orderRepository.GetAllOrders(
                    start, length, sortColumn, sortColumnDir, searchValue);

                CommonViewModel.IsSuccess = true;
                CommonViewModel.StatusCode = ResponseStatusCode.Success;
                CommonViewModel.Data = data;
            }
            catch (Exception ex)
            {
                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;
            }

            return Ok(CommonViewModel);
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save(OrderCombo model)
        {


            try
            {

                //var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                //var indiaToday = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaTimeZone).Date;
                if (model.Order.CustomerId == null || model.Order.CustomerId <= 0)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please Select Customer";
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    return Ok(CommonViewModel);
                }

                if (string.IsNullOrWhiteSpace(model.Order.Order_No))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Invoice number is required";
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    return Ok(CommonViewModel);
                }

                if (model.Order.Order_Date == null)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Order date is required";
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    return Ok(CommonViewModel);
                }
                //else if (model.Order.Order_Date > indiaToday)
                //{
                //    CommonViewModel.IsSuccess = false;
                //    CommonViewModel.Message = "Order date must be in the past";
                //    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                //    return Ok(CommonViewModel);
                //}
                if (model.OrderItems == null || model.OrderItems.Count == 0)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "At least one item is required";
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    return Ok(CommonViewModel);
                }               
                if (model.Order.Total_Amount == null || model.Order.Total_Amount < 0)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Total amount must be valid";
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    return Ok(CommonViewModel);
                }

                if (string.IsNullOrWhiteSpace(model.Order.Order_Status))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Status is required";
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    return Ok(CommonViewModel);
                }

                var (IsSuccess, Message, Id, Extra) = await _orderRepository.AddOrUpdateOrder(model);

                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.IsConfirm = true;
                CommonViewModel.StatusCode = IsSuccess ? ResponseStatusCode.Success : ResponseStatusCode.Error;
                CommonViewModel.Message = Message;
                CommonViewModel.Data = Id;


            }
            catch (Exception ex)
            {
                string actionName = ControllerContext.RouteData.Values["action"]?.ToString();
                string controllerName = ControllerContext.RouteData.Values["controller"]?.ToString();
                string requestUrl = $"{Request?.Scheme}://{Request?.Host}{Request?.Path}{Request?.QueryString}";
                string userAgent = Request?.Headers["User-Agent"].ToString(); //broser name and servion
                string clientIp = HttpContext.Connection?.RemoteIpAddress?.ToString();
                string requestPayload = System.Text.Json.JsonSerializer.Serialize(model);
                long? userId = AppHttpContextAccessor.JwtUserId;
                var log = new ErrorLog
                {
                    ApplicationName = AppHttpContextAccessor.ApplicationName,
                    ControllerName = controllerName + "_" + actionName,
                    ErrorMessage = ex.Message,
                    ErrorType = ex.GetType().FullName,
                    StackTrace = ex.ToString(),
                    RequestUrl = requestUrl,
                    RequestPayload = requestPayload,
                    UserAgent = userAgent,
                    UserId = userId,
                    ClientIP = clientIp,

                    CreatedBy = User?.Identity?.Name
                };

                LogEntry.InsertLogEntry(log);
                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;
                //  return Ok(CommonViewModel);
            }
            return Ok(CommonViewModel);
        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var data = await _orderRepository.GetOrderById(id);

                if (data != null)
                {
                    CommonViewModel.IsSuccess = true;
                    CommonViewModel.StatusCode = ResponseStatusCode.Success;
                    CommonViewModel.Data = data;
                }
                else
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.NotFound;
                    CommonViewModel.Message = ResponseStatusMessage.NotFound;
                }
            }
            catch (Exception ex)
            {
                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;
            }

            return Ok(CommonViewModel);
        }
        [HttpGet("[Action]")]
        public async Task<IActionResult> GetOrderNo()
        {
            try
            {
                var data = await _orderRepository.GetOrderNo();

                if (data != null)
                {
                    CommonViewModel.IsSuccess = true;
                    CommonViewModel.StatusCode = ResponseStatusCode.Success;
                    CommonViewModel.Data = data;
                }
                else
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.NotFound;
                    CommonViewModel.Message = ResponseStatusMessage.NotFound;
                }
            }
            catch (Exception ex)
            {
                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;
            }

            return Ok(CommonViewModel);
        }
        [HttpDelete("[Action]")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var (IsSuccess, Message, Id, Extra) = await _orderRepository.DeleteByOrderId(id);
                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.IsConfirm = true;
                CommonViewModel.StatusCode = IsSuccess ? ResponseStatusCode.Success : ResponseStatusCode.Error;
                CommonViewModel.Message = Message;
                CommonViewModel.Data = Id;
            }
            catch (Exception ex)
            {
                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;
            }

            return Ok(CommonViewModel);
        }

        [HttpGet("[Action]")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrderLayoutdata(long id, string status = "")
        {
            var data = await _orderRepository.GetOrderLayoutdata(id, status);

            if (data != null)
            {
                CommonViewModel.IsSuccess = true;
                CommonViewModel.StatusCode = ResponseStatusCode.Success;
                CommonViewModel.Data = data;
            }
            else
            {
                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.NotFound;
                CommonViewModel.Message = "Order not found";
            }

            return Ok(CommonViewModel);
        }
    }

}

