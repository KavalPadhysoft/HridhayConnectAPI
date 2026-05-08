using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.UserRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HridhayConnect_API.Infra;
using Microsoft.AspNetCore.Identity.Data;
using System.Text.Json.Serialization;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace HridhayConnect_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly CommonViewModel CommonViewModel = new();
        private readonly ValidationService _validation;


        public UserController(IUserRepository userRepository, ValidationService validation)
        {
            _userRepository = userRepository;
            _validation = validation;
        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> GetAllpage(int start = 0, int length = 10, string sortColumn = "", string sortColumnDir = "asc", string searchValue = "")
        {
            try
            {
                var data = await _userRepository.GetAllUsers(start, length, sortColumn, sortColumnDir, searchValue);
                CommonViewModel.IsSuccess = true;
                CommonViewModel.StatusCode = ResponseStatusCode.Success;
                CommonViewModel.Data = data;
            }
            catch (Exception ex)
            {
                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;

                string actionName = ControllerContext.RouteData.Values["action"]?.ToString();
                string controllerName = ControllerContext.RouteData.Values["controller"]?.ToString();
                string requestUrl = $"{Request?.Scheme}://{Request?.Host}{Request?.Path}{Request?.QueryString}";
                string userAgent = Request?.Headers["User-Agent"].ToString();
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


        [HttpGet("ExportToExcel")]
        public async Task<IActionResult> ExportToExcel( int start = 0,int length = 1000,string sortColumn = "", string sortColumnDir = "asc", string searchValue = "" )
        {
            try
            {
                var result = await _userRepository.GetAllUsers(
                    start, length, sortColumn, sortColumnDir, searchValue
                );

                // ✅ EPPlus 8 License सेट करो
                ExcelPackage.License.SetNonCommercialPersonal("Yash");

                using (var package = new ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Users");

                    // ✅ Header
                    sheet.Cells[1, 1].Value = "User Name";
                    sheet.Cells[1, 2].Value = "Email";
                    sheet.Cells[1, 3].Value = "Mobile";
                    sheet.Cells[1, 4].Value = "Role";

                    // ✅ Correct range (4 columns only)
                    sheet.Cells["A1:D1"].Style.Font.Bold = true;

                    int row = 2;

                    // ✅ Data bind
                    if (result?.Data != null)
                    {
                        foreach (var item in result.Data)
                        {
                            sheet.Cells[row, 1].Value = item.UserName;
                            sheet.Cells[row, 2].Value = item.Email;
                            sheet.Cells[row, 3].Value = item.MobileNumber;
                            sheet.Cells[row, 4].Value = item.Rolename;
                            row++;
                        }
                    }

                    // ✅ Auto fit
                    sheet.Cells["A:D"].AutoFitColumns();

                    using (var stream = new MemoryStream())
                    {
                        package.SaveAs(stream);
                        stream.Position = 0;

                        return File(
                            stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "Users.xlsx"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("ExportToPdf")]
        public async Task<IActionResult> ExportToPdf(
           int start = 0,
           int length = 1000,
           string sortColumn = "",
           string sortColumnDir = "asc",
           string searchValue = ""
       )
        {
            try
            {
                var result = await _userRepository.GetAllUsers(
                    start, length, sortColumn, sortColumnDir, searchValue
                );

                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

                var pdf = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(20);
                        page.Size(PageSizes.A4);

                        // ✅ Header
                        page.Header()
                            .Text("User Report")
                            .Bold()
                            .FontSize(18)
                            .AlignCenter();

                        page.Content().PaddingTop(10).Table(table =>
                        {
                            // ✅ Column width control
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Name
                                columns.RelativeColumn(3); // Email
                                columns.RelativeColumn(2); // Mobile
                                columns.RelativeColumn(2); // Role
                            });

                            // ✅ HEADER STYLE
                            table.Header(header =>
                            {
                                HeaderCell(header, "User Name");
                                HeaderCell(header, "Email");
                                HeaderCell(header, "Mobile");
                                HeaderCell(header, "Role");
                            });

                            // ✅ DATA
                            if (result?.Data != null)
                            {
                                foreach (var item in result.Data)
                                {
                                    BodyCell(table, item.UserName);
                                    BodyCell(table, item.Email);
                                    BodyCell(table, item.MobileNumber);
                                    BodyCell(table, item.Rolename);
                                }
                            }
                        });
                    });
                }).GeneratePdf();

                return File(pdf, "application/pdf", "Users.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("[Action]")]
        public async Task<IActionResult> Add(User user)
        {
            try
            {
                var validation = _validation.ValidateRequired(user.UserName, "User Name");
                if (!validation.IsSuccess) { return Ok(validation); }
                if (user.Id == 0)
                {
                    var password = _validation.ValidateRequired(user.Password, "Password");
                    if (!password.IsSuccess)
                    { return Ok(password); 
                    }
                }
    


                var email = _validation.ValidateEmail(user.Email);
                if (!email.IsSuccess) { return Ok(email); }

                var mobileValidation = _validation.ValidateMobile(user.MobileNumber);
                if (!mobileValidation.IsSuccess)   {    return Ok(mobileValidation); }

                var RoleId = _validation.ValidateDropdown_Long(user.RoleId, "Role name");
                if (!RoleId.IsSuccess) { return Ok(RoleId); }

                var (IsSuccess, Message, Id, Extra) = await _userRepository.AddOrUpdateUser(user);
                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.IsConfirm = true;
                CommonViewModel.StatusCode = IsSuccess ? ResponseStatusCode.Success : ResponseStatusCode.Error;
                CommonViewModel.Message = Message;
                CommonViewModel.Data = Id;
                //    return await _userRepository.AddOrUpdateUser(user);

            }
            catch (Exception ex)
            {
                string actionName = ControllerContext.RouteData.Values["action"]?.ToString();
                string controllerName = ControllerContext.RouteData.Values["controller"]?.ToString();
                string requestUrl = $"{Request?.Scheme}://{Request?.Host}{Request?.Path}{Request?.QueryString}";
                string userAgent = Request?.Headers["User-Agent"].ToString(); //broser name and servion
                string clientIp = HttpContext.Connection?.RemoteIpAddress?.ToString();
                string requestPayload = System.Text.Json.JsonSerializer.Serialize(user);
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
                var data = await _userRepository.GetUserById(id);
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

                string actionName = ControllerContext.RouteData.Values["action"]?.ToString();
                string controllerName = ControllerContext.RouteData.Values["controller"]?.ToString();
                string requestUrl = $"{Request?.Scheme}://{Request?.Host}{Request?.Path}{Request?.QueryString}";
                string userAgent = Request?.Headers["User-Agent"].ToString(); //broser name and servion
                string clientIp = HttpContext.Connection?.RemoteIpAddress?.ToString();
                	string requestPayload = System.Text.Json.JsonSerializer.Serialize(new{id});
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
                return null;

            }
            return Ok(CommonViewModel);
        }

        [HttpDelete("[Action]")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var (IsSuccess, Message, Id, Extra) = await _userRepository.DeletebyUserId(id);
                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.IsConfirm = true;
                CommonViewModel.StatusCode = IsSuccess ? ResponseStatusCode.Success : ResponseStatusCode.Error;
                CommonViewModel.Message = Message;
                CommonViewModel.Data = Id;
            }
            catch(Exception ex)
            {
                string actionName = ControllerContext.RouteData.Values["action"]?.ToString();
                string controllerName = ControllerContext.RouteData.Values["controller"]?.ToString();
                string requestUrl = $"{Request?.Scheme}://{Request?.Host}{Request?.Path}{Request?.QueryString}";
                string userAgent = Request?.Headers["User-Agent"].ToString(); //broser name and servion
                string clientIp = HttpContext.Connection?.RemoteIpAddress?.ToString();
                string requestPayload = System.Text.Json.JsonSerializer.Serialize(new
                {
                    id
                });
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
               
            }
            return Ok(CommonViewModel);
        }


        [HttpPost("[action]")]
     
        public async Task<IActionResult> ResetPassword(string username)
        {
            if (username == null || string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { Response = "E|Invalid input|0" });
            }

            try
            {
                var (IsSuccess, Message, Id, Extra) = await _userRepository.ResetPasswordByUsername(username.Trim());
                // result is JsonResult already (to keep parity with your repo). Return its value.
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
                string requestPayload = JsonConvert.ToString(username);
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
               
            }
            return Ok(CommonViewModel);
        }


        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> forgotpassword(string username)
        {
            if (username == null || string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { Response = "E|Invalid input|0" });
            }

            try
            {
                var result = await _userRepository.ForgetPassword(username.Trim());
                // result is JsonResult already (to keep parity with your repo). Return its value.
                return result;
            }
            catch (Exception ex)
            {
                string actionName = ControllerContext.RouteData.Values["action"]?.ToString();
                string controllerName = ControllerContext.RouteData.Values["controller"]?.ToString();
                string requestUrl = $"{Request?.Scheme}://{Request?.Host}{Request?.Path}{Request?.QueryString}";
                string userAgent = Request?.Headers["User-Agent"].ToString(); //broser name and servion
                string clientIp = HttpContext.Connection?.RemoteIpAddress?.ToString();
                string requestPayload = JsonConvert.ToString(username);
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
                return Ok(CommonViewModel);
            }
        }



        [HttpPost("[Action]")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword_GenerateOTP( string Email)
        {
            try
            {
                if (string.IsNullOrEmpty(Email))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    CommonViewModel.Message = "Email is required";

                    return Ok(CommonViewModel);
                }

                var data = await _userRepository.ForgotPassword_GenerateOTP(Email);

                if (data != null && data.Status == 1)
                {
                    CommonViewModel.IsSuccess = true;
                    CommonViewModel.StatusCode = ResponseStatusCode.Success;
                    CommonViewModel.Message = data.Message;
                    CommonViewModel.Data = null;
                    var dict = data.Data as IDictionary<string, object>;
                    var newOtp = dict?["OTP"]?.ToString();

                    Common.SendEmail(
                        "Your OTP Code",
                        Email,
                        true,
                        "",
                        "otp_message",
                        JsonConvert.SerializeObject(new { otp = newOtp })
                    );
                }
                else
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.NotFound;
                    CommonViewModel.Message = data?.Message ?? "Invalid Email";
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


        [HttpPost("[Action]")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword_VerifyOTP(string Email, int Otp)
        {
            try
            {
                if (Otp == 0)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    CommonViewModel.Message = "otp is required";

                    return Ok(CommonViewModel);
                }

                var data = await _userRepository.ForgotPassword_VerifyOTP(Email, Otp);

                if (data != null && data.Status == 1)
                {
                    CommonViewModel.IsSuccess = true;
                    CommonViewModel.StatusCode = ResponseStatusCode.Success;
                    CommonViewModel.Message = data.Message;
                    CommonViewModel.Data = data.Data;
                }
                else
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.NotFound;
                    CommonViewModel.Message = data?.Message ?? "Invalid otp";
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


        [HttpPost("[Action]")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword_ResetPassword(string Email, string NewPassword, string ConfirmPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(NewPassword))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    CommonViewModel.Message = "New Password is required.";

                    return Ok(CommonViewModel);
                }
                if (string.IsNullOrEmpty(ConfirmPassword))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    CommonViewModel.Message = "Confirm Password is required.";

                    return Ok(CommonViewModel);
                }
                if (NewPassword != ConfirmPassword)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    CommonViewModel.Message = "Passwords do not match.";

                    return Ok(CommonViewModel);
                }
                var data = await _userRepository.ForgotPassword_ResetPassword(Email, NewPassword);

                if (data != null && data.Status == 1)
                {
                    CommonViewModel.IsSuccess = true;
                    CommonViewModel.StatusCode = ResponseStatusCode.Success;
                    CommonViewModel.Message = data.Message;
                    CommonViewModel.Data = data.Data;
                }
                else
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.NotFound;
                    CommonViewModel.Message = data?.Message ?? "Invalid otp";
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



        void HeaderCell(TableCellDescriptor header, string text)
        {
            header.Cell().Element(c => c
                .Border(1)
                .Background(Colors.Grey.Lighten2)
                .Padding(5)
                .AlignCenter()
                .Text(text ?? "")
                .Bold()
            );
        }

        void BodyCell(TableDescriptor table, string text)
        {
            table.Cell().Element(c => c
                .Border(1)
                .Padding(5)
                .Text(text ?? "")
            );
        }

    }
}
