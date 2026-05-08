using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HridhayConnect_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly CommonViewModel _commonResponse = new();
        private readonly IRepositoryBase<User> _repositoryBase;



        public AuthController(DataContext context, IConfiguration configuration, IRepositoryBase<User> repositoryBase)
        {
            _context = context;
            _configuration = configuration;
            _repositoryBase = repositoryBase;
        }

        [HttpPost("[Action]")]
        public async Task<IActionResult> Login(string userName, string password)
        {
            try
            {
                var haspassword = Common.Encrypt(password);


                var outputParam = new SqlParameter("@response", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };

                var parameters = new[]

                {
                   new SqlParameter("@puserid", userName),
                   new SqlParameter("@password", haspassword),
                   outputParam
                };


                _repositoryBase.ExecuteStoredProcedure("CheckUserAuthenticationForUserMaster", parameters);
                string response = outputParam.Value?.ToString() ?? "{}";

                if (string.IsNullOrEmpty(response))
                {
                    _commonResponse.IsSuccess = false;
                    _commonResponse.StatusCode = ResponseStatusCode.Error;
                    _commonResponse.Message = "No response from stored procedure.";
                    return Ok(_commonResponse);
                }

                // Expected response format: S|Message|UserId|RoleId
                var parts = response.Split('|', StringSplitOptions.RemoveEmptyEntries);
                string msgType = parts.ElementAtOrDefault(0) ?? "E";
                string message = parts.ElementAtOrDefault(1) ?? "Unknown error";
                string userId = parts.ElementAtOrDefault(2) ?? "0";
                string username = parts.ElementAtOrDefault(3);
                string roleId = parts.ElementAtOrDefault(4);
                string roleName = parts.ElementAtOrDefault(5);



                if (msgType == "S")
                {
                    var user = new User
                    {
                        Id = Convert.ToInt32(userId),
                        UserName = userName,
                        RoleId = Convert.ToInt32(roleId),
                        Rolename = roleName

                    };

                    var token = GenerateJwtToken(user);

                    _commonResponse.IsSuccess = true;
                    _commonResponse.StatusCode = ResponseStatusCode.Success;
                    //_commonResponse.Data = new
                    //{
                    //    Token = token,
                    //    UserId = user.Id,
                    //    UserName = user.UserName
                    //};

                    // var message2 = $"{msgType}|{message}|{token}";

                    _commonResponse.Message = message;
                    _commonResponse.Data = token;
                }
                else
                {
                    _commonResponse.IsSuccess = false;
                    _commonResponse.StatusCode = ResponseStatusCode.Error;
                  //  var Emessage = $"E|{message}|0";
                    _commonResponse.Message = message;
                }
            }
            catch (Exception ex)
            {
                _commonResponse.IsSuccess = false;
                _commonResponse.StatusCode = ResponseStatusCode.Error;
                _commonResponse.Message = ex.Message;

                string actionName = ControllerContext.RouteData.Values["action"]?.ToString();
                string controllerName = ControllerContext.RouteData.Values["controller"]?.ToString();
                string requestUrl = $"{Request?.Scheme}://{Request?.Host}{Request?.Path}{Request?.QueryString}";
                string userAgent = Request?.Headers["User-Agent"].ToString(); //broser name and servion
                string clientIp = HttpContext.Connection?.RemoteIpAddress?.ToString();
                string requestPayload = JsonConvert.ToString(userName);
                long? userId = null;
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
                    CreatedBy = userName
                };

                LogEntry.InsertLogEntry(log);
            }

            return Ok(_commonResponse);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("RoleId", user.RoleId.ToString()),
                new Claim("RoleName", user.Rolename)
             };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSettings["DurationInDays"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



    }
}
