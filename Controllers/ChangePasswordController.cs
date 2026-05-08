using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HridhayConnect_API.Infra;
using HridhayConnect_API.ServiceRepository.ChangePasswordRepository;
using HridhayConnect_API.Models;

namespace HridhayConnect_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChangePasswordController : ControllerBase
    {
        private readonly IChangePasswordRepository _changePasswordRepository;
        private readonly CommonViewModel CommonViewModel = new();
        private readonly ValidationService _validation;
        public ChangePasswordController(IChangePasswordRepository changePasswordRepository, ValidationService validation)
        {
            _changePasswordRepository = changePasswordRepository;
            _validation = validation;
        }


        [HttpPost("[Action]")]
        public async Task<IActionResult> Add(ChangePassword changePassword)
        {
            try
            {
                var OldPassword = _validation.ValidateRequired(changePassword.OldPassword, "Old Password");
                if (!OldPassword.IsSuccess) { return Ok(OldPassword); }

                var NewPassword = _validation.ValidateRequired(changePassword.NewPassword, "New Password");
                if (!NewPassword.IsSuccess) { return Ok(NewPassword); }

                var ConfirmPassword = _validation.ValidateRequired(changePassword.ConfirmPassword, "Confirm Password");
                if (!ConfirmPassword.IsSuccess) { return Ok(ConfirmPassword); }

              

                if (changePassword.NewPassword != changePassword.ConfirmPassword)
                {
                    CommonViewModel.Message = "Confirm Password is not matching with New Password";
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    return Ok(CommonViewModel);
                }

                var (IsSuccess, Message, Id, Extra) = await _changePasswordRepository.ChangePassword(changePassword);

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
                string requestPayload = JsonConvert.SerializeObject(changePassword);
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
                CommonViewModel.StatusCode = ResponseStatusCode.NotFound;
                CommonViewModel.Message = ex.Message;


            }
            return Ok(CommonViewModel);
        }


    }
}
