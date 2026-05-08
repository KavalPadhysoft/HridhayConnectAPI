using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.DeliveriesRepository;
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
    public class DeliveriesController : ControllerBase
    {
        private readonly IDeliveriesRepository _deliveriesRepository;
        private readonly CommonViewModel CommonViewModel = new();
        private readonly ValidationService _validation;

        public DeliveriesController(IDeliveriesRepository deliveriesRepository, ValidationService validation)
        {
            _deliveriesRepository = deliveriesRepository;
            _validation = validation;
        }

       

        [HttpPost("Save")]
        public async Task<IActionResult> Save(DeliveriesCombo model)
        {


            try
            {

                //var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                //var indiaToday = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaTimeZone).Date;
             
                if (model.Order.Delivery_Date == null)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Delivery date is required";
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    return Ok(CommonViewModel);
                }
                //else if (model.Order.Delivery_Date > indiaToday)
                //{
                //    CommonViewModel.IsSuccess = false;
                //    CommonViewModel.Message = "Delivery date must be in the past";
                //    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                //    return Ok(CommonViewModel);
                //}
                if (model.DeliveryItems == null || model.DeliveryItems.Count == 0)
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
               

                var (IsSuccess, Message, Id, Extra) = await _deliveriesRepository.AddOrUpdateDelivery(model);

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
                var data = await _deliveriesRepository.GetDeliveriesById(id);

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
        public async Task<IActionResult> CancelOrder(long id)
        {
            try
            {
                var (IsSuccess, Message, Id, Extra) = await _deliveriesRepository.CancelOrder(id);
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
    }

}

