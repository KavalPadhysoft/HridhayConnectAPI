
using HridhayConnect_API.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace HridhayConnect_API.ServiceRepository.ChangePasswordRepository
{
    public interface IChangePasswordRepository
    {

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> ChangePassword(ChangePassword changePassword);
    }

    public class ChangePasswordRepository : IChangePasswordRepository
    {
        private readonly DataContext _context;
        private readonly CommonViewModel CommonViewModel = new();
        private readonly IRepositoryBase<ChangePassword> _repositoryBase;

        public ChangePasswordRepository(DataContext context, IRepositoryBase<ChangePassword> repositoryBase)
        {
            _context = context;
            _repositoryBase = repositoryBase;
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> ChangePassword(ChangePassword ViewModel)
        {
            try { 
          


                List<SqlParameter> parameters = new();

            var oldpaasword = Common.Encrypt(ViewModel.OldPassword);
            var ConfirmPassword = Common.Encrypt(ViewModel.ConfirmPassword);




            parameters.Add(new SqlParameter("ConfirmPassword", ConfirmPassword));
            parameters.Add(new SqlParameter("OldPassword", oldpaasword));
            parameters.Add(new SqlParameter("Operated_By", AppHttpContextAccessor.JwtUserId));


            var result=_repositoryBase.ExecuteStoredProcedurenew("ChangePassword", parameters,true);

            return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
