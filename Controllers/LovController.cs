using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.LovRepository;

namespace HridhayConnect_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LovController : ControllerBase
    {
        private readonly ILovRepository _lovRepository;
        private readonly CommonViewModel CommonViewModel = new();

        public LovController(ILovRepository lovRepository)
        {
            _lovRepository = lovRepository;
        }
        [HttpGet("[Action]")]
        public async Task<IActionResult> Get(
            string flag,
            string lovColumn = "",
            string lovCode = "")
        {
            try
            {
                var data = await _lovRepository.GetLov(lovColumn, lovCode, flag);

                CommonViewModel.IsSuccess = true;
                CommonViewModel.StatusCode = ResponseStatusCode.Success;
                CommonViewModel.Data = data;
            }
            catch (Exception ex)
            {
                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;

                LogError(ex, new { flag, lovColumn, lovCode });
            }

            return Ok(CommonViewModel);
        }

        [HttpPost("[Action]")]
        public async Task<IActionResult> SaveMaster([FromBody] LovMaster model)
        {
            try
            {
                var (IsSuccess, Message, Id, Extra) = await _lovRepository.SaveLovMaster(model);
                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.IsConfirm = true;
                CommonViewModel.StatusCode = IsSuccess ? ResponseStatusCode.Success : ResponseStatusCode.Error;
                CommonViewModel.Message = Message;
                CommonViewModel.Data = Id;
            }
            catch (Exception ex)
            {
                LogError(ex, model);

                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;
                string actionName = ControllerContext.RouteData.Values["action"]?.ToString();
                string controllerName = ControllerContext.RouteData.Values["controller"]?.ToString();
                string requestUrl = $"{Request?.Scheme}://{Request?.Host}{Request?.Path}{Request?.QueryString}";
                string userAgent = Request?.Headers["User-Agent"].ToString(); //broser name and servion
                string clientIp = HttpContext.Connection?.RemoteIpAddress?.ToString();
                string requestPayload = null;
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

            }
            return Ok(CommonViewModel);
        }

        [HttpPost("[Action]")]
        public async Task<IActionResult> SaveDetail([FromBody] LovMaster model)
        {
            try
            {
                var (IsSuccess, Message, Id, Extra) = await _lovRepository.SaveLovDetail(model);
                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.IsConfirm = true;
                CommonViewModel.StatusCode = IsSuccess ? ResponseStatusCode.Success : ResponseStatusCode.Error;
                CommonViewModel.Message = Message;
                CommonViewModel.Data = Id;
            }
            catch (Exception ex)
            {
                LogError(ex, model);

                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;


            }
            return Ok(CommonViewModel);
        }
        [HttpDelete("[Action]")]
        public async Task<IActionResult> DeleteDetail(
            string lovColumn,
            string? lovCode)
        {
            try
            {
                var (IsSuccess, Message, Id, Extra) = await _lovRepository.DeleteLovDetail(
                    lovColumn, lovCode);

                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.IsConfirm = true;
                CommonViewModel.StatusCode = IsSuccess ? ResponseStatusCode.Success : ResponseStatusCode.Error;
                CommonViewModel.Message = Message;
                CommonViewModel.Data = Id;
            }
            catch (Exception ex)
            {
                LogError(ex, new { lovColumn, lovCode });

                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;


            }
            return Ok(CommonViewModel);
        }

        [HttpDelete("[Action]")]
        public async Task<IActionResult> DeleteMaster(
    string lovColumn)
        {
            try
            {
                var (IsSuccess, Message, Id, Extra) = await _lovRepository.DeleteLovMaster(
                    lovColumn);

                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.IsConfirm = true;
                CommonViewModel.StatusCode = IsSuccess ? ResponseStatusCode.Success : ResponseStatusCode.Error;
                CommonViewModel.Message = Message;
                CommonViewModel.Data = Id;
            }
            catch (Exception ex)
            {
                LogError(ex, new { lovColumn});

                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;


            }
            return Ok(CommonViewModel);
        }


        private void LogError(Exception ex, object payload)
        {
            string actionName = ControllerContext.RouteData.Values["action"]?.ToString();
            string controllerName = ControllerContext.RouteData.Values["controller"]?.ToString();
            string requestUrl = $"{Request?.Scheme}://{Request?.Host}{Request?.Path}{Request?.QueryString}";
            string userAgent = Request?.Headers["User-Agent"].ToString();
            string clientIp = HttpContext.Connection?.RemoteIpAddress?.ToString();
            string requestPayload = System.Text.Json.JsonSerializer.Serialize(payload);
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
        }
    }
}
