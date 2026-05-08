using HridhayConnect_API.ServiceRepository.MenuAccessRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HridhayConnect_API.Infra;

namespace HridhayConnect_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuAccessController : ControllerBase
    {
        private readonly IMenuAccessRepo _menuRepository;

        public MenuAccessController(IMenuAccessRepo menuRepository)
        {
            _menuRepository = menuRepository;
        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> GetMenuAccess()
        {
            try
            {
                var data = await _menuRepository.GetMenuAccessAsync();

                if (data == null || data.Count == 0)
                    return NotFound(new { message = "No menu access found." });

                return Ok(data);
            }
            catch (Exception ex)
            {

                string actionName = ControllerContext.RouteData.Values["action"]?.ToString();
                string controllerName = ControllerContext.RouteData.Values["controller"]?.ToString();
                string requestUrl = $"{Request?.Scheme}://{Request?.Host}{Request?.Path}{Request?.QueryString}";
                string userAgent = Request?.Headers["User-Agent"].ToString(); //broser name and servion
                string clientIp = HttpContext.Connection?.RemoteIpAddress?.ToString();
                string requestPayload = null;
                //long? userId = AppHttpContextAccessor.JwtUserId;
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
                    UserId = AppHttpContextAccessor.JwtUserId,
                    ClientIP = clientIp,

                    CreatedBy = User?.Identity?.Name
                };

                LogEntry.InsertLogEntry(log);
               

                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}