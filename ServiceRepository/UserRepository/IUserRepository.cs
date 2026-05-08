using HridhayConnect_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HridhayConnect_API.Infra;
using HridhayConnect_API.Infra;

namespace HridhayConnect_API.ServiceRepository.UserRepository
{
    public interface IUserRepository
    {
        Task<PagedResult<User>> GetAllUsers(int start, int length, string sortColumn, string sortColumnDir, string searchValue);
        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdateUser(User role);
        Task<User?> GetUserById(long id);
        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeletebyUserId(long id);


        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> ResetPasswordByUsername(string username);
        Task<JsonResult> ForgetPassword(string username);

        Task<LoginResult?> ForgotPassword_GenerateOTP(string email);
        Task<LoginResult?> ForgotPassword_VerifyOTP(string email, int otp);
        Task<LoginResult?> ForgotPassword_ResetPassword(string email, string newPassword);
    }
}
