using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Net.Mail;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace HridhayConnect_API.ServiceRepository.UserRepository
{
    public class UserRepository : IUserRepository
    {

        private readonly DataContext _context;
        private readonly IRepositoryBase<User> _repositoryBase;
        private readonly CommonViewModel CommonViewModel = new();
        public UserRepository(DataContext context, IRepositoryBase<User> repositoryBase)
        {
            _context = context;
            _repositoryBase = repositoryBase;
        }
        public async Task<PagedResult<User>> GetAllUsers(
          int start,
          int length,
          string sortColumn,
          string sortColumnDir,
          string searchValue)

        {
            try
            {
                SqlParameter[] parameters = null;

                var pagedResult = _repositoryBase.ExecuteWithPagination(
                    "sp_Users_Get",
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

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdateUser(User user)
        {
            try
            {
                List<SqlParameter> oParams = new();

                string encryptedPassword = Common.Encrypt(user.Password ?? "");

                oParams.Add(new SqlParameter("@UserId", user.Id));
                oParams.Add(new SqlParameter("@Username", user.UserName ?? (object)DBNull.Value));
                oParams.Add(new SqlParameter("@Password_hash", encryptedPassword));
                oParams.Add(new SqlParameter("@Mobile_No", user.MobileNumber ?? (object)DBNull.Value));
                oParams.Add(new SqlParameter("@Email_id", user.Email ?? (object)DBNull.Value));
                oParams.Add(new SqlParameter("@Operated_By", AppHttpContextAccessor.JwtUserId));
                oParams.Add(new SqlParameter("@Operated_RoleId", user.RoleId ?? (object)DBNull.Value));
                oParams.Add(new SqlParameter("@Action", user.Id == 0 ? "INSERT" : "UPDATE"));

                // var result = DataContext.ExecuteStoredProcedurenew("sp_User_Save", oParams, true);

                //return await Task.FromResult(result);

                // _repositoryBase.ExecuteStoredProcedurenew("sp_User_Save", oParams, true);
                var result = _repositoryBase.ExecuteStoredProcedurenew("sp_User_Save", oParams, true);

                //var responseJson = outputParam.Value?.ToString() ?? "{}";
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<User?> GetUserById(long id)
        {
            try
            {
                var parameters = new[]
                {
         new SqlParameter("@user_id", id)
     };

                var result = _repositoryBase.ExecuteSingle(
                    "sp_Users_Get",
                    parameters
                );

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeletebyUserId(long id)
        {

            try
            {
                List<SqlParameter> parameters = new()

                {
                    new SqlParameter("UserId", id),
                    new SqlParameter("Operated_By",AppHttpContextAccessor.JwtUserId)
                };
                var result = _repositoryBase.ExecuteStoredProcedurenew("sp_User_Delete", parameters, true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> ResetPasswordByUsername(string username)
        {
            try
            {
                var temporaryPassword = AppHttpContextAccessor.Defaultpassword;
                var encryptedPassword = Common.Encrypt(temporaryPassword);

                List<SqlParameter> oParams = new()
        {
            new SqlParameter("@Username", username ?? (object)DBNull.Value),
            new SqlParameter("@NewPasswordHash", encryptedPassword ?? (object)DBNull.Value)
        };

                var result = _repositoryBase.ExecuteStoredProcedurenew("ResetPasswordByUsername", oParams, true);

                if (result.IsSuccess)
                {
                    var email = result.Extra?.FirstOrDefault();

                    var templateJson = JsonConvert.SerializeObject(new
                    {
                        otp = temporaryPassword
                    });

                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        await Common.SendEmail(
                            "Your Password",
                            email,
                            true,
                            "",
                            "otp_message",
                            templateJson);
                    }

                    return result;
                }

                return result;
            }
            catch (Exception)
            {
                // TODO: add logging here
                throw;
            }
        }
        public async Task<JsonResult> ForgetPassword(string username)
        {
            try
            {

                var tempPasswordPlain = AppHttpContextAccessor.Defaultpassword;


                var encryptedPassword = Common.Encrypt(tempPasswordPlain);

                var outputParam = new SqlParameter("@response", SqlDbType.NVarChar, -1)
                {
                    Direction = ParameterDirection.Output
                };

                var parameters = new[]
                {
                 new SqlParameter("@Username", username ?? (object)DBNull.Value),
                 new SqlParameter("@NewPasswordHash", encryptedPassword ?? (object)DBNull.Value),
                 outputParam
                };

                _repositoryBase.ExecuteStoredProcedure("Forgotpassword", parameters);


                var response = (outputParam.Value?.ToString() ?? "E|Unexpected error|0").Trim();
                var parts = response.Split(new[] { '|' }, StringSplitOptions.None);

                // defaults
                var status = parts.Length > 0 ? parts[0] : "E";
                var messageText = parts.Length > 1 ? parts[1] : "Unexpected response";
                var idString = parts.Length > 2 ? parts[2] : "0";

                if (!string.IsNullOrEmpty(response) && status == "S")
                {
                    var templateJson = JsonConvert.SerializeObject(new
                    {
                        otp = tempPasswordPlain
                    });
                    await Common.SendEmail(
                      "Your Password",
                      idString,
                      true,
                       "",
                      "otp_message",
                       templateJson);
                    return new JsonResult(response);
                }

                return new JsonResult(new { Response = response });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<LoginResult> ForgotPassword_GenerateOTP(string email)
        {
            try
            {
                LoginResult result = new LoginResult();

                var oParams = new List<SqlParameter>()
                {
                    new SqlParameter("@Email", email)
                };

                DataTable dt = _repositoryBase.ExecuteStoredProcedureDataTable("SP_ForgotPassword_GenerateOTP", oParams);

                if (dt != null && dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];

                    result.Status = Convert.ToInt32(row["Status"]);
                    result.Message = row["Message"].ToString();

                    if (result.Status == 1)
                    {
                        var dict = new Dictionary<string, object>();

                        foreach (DataColumn col in row.Table.Columns)
                        {
                            dict[col.ColumnName] = row[col];
                        }

                        result.Data = dict;
                    }
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<LoginResult> ForgotPassword_VerifyOTP(string email, int otp)
        {
            try
            {
                LoginResult result = new LoginResult();

                var oParams = new List<SqlParameter>()
                {
                    new SqlParameter("@Email", email),
                    new SqlParameter("@OTP", otp)
                };

                DataTable dt = _repositoryBase.ExecuteStoredProcedureDataTable("SP_ForgotPassword_VerifyOTP", oParams);

                if (dt != null && dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];

                    result.Status = Convert.ToInt32(row["Status"]);
                    result.Message = row["Message"].ToString();

                    if (result.Status == 1)
                    {
                        var dict = new Dictionary<string, object>();

                        foreach (DataColumn col in row.Table.Columns)
                        {
                            dict[col.ColumnName] = row[col];
                        }

                        result.Data = dict;
                    }
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<LoginResult> ForgotPassword_ResetPassword(string email, string newPassword)
        {
            try
            {
                LoginResult result = new LoginResult();

                var Password = Common.Encrypt(newPassword);

                var oParams = new List<SqlParameter>()
                {
                    new SqlParameter("@Email", email),
                    new SqlParameter("@NewPassword", Password)
                };

                DataTable dt = _repositoryBase.ExecuteStoredProcedureDataTable("SP_ForgotPassword_ResetPassword", oParams);

                if (dt != null && dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];

                    result.Status = Convert.ToInt32(row["Status"]);
                    result.Message = row["Message"].ToString();

                    if (result.Status == 1)
                    {
                        var dict = new Dictionary<string, object>();

                        foreach (DataColumn col in row.Table.Columns)
                        {
                            dict[col.ColumnName] = row[col];
                        }

                        result.Data = dict;
                    }
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
