using Microsoft.AspNetCore.Mvc;
using HridhayConnect_API.Models;

namespace HridhayConnect_API.ServiceRepository.LovRepository
{
    public interface ILovRepository
    {

        Task<List<LovMaster>> GetLov(string lovColumn, string lovCode, string flag);
        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveLovMaster(LovMaster model);
        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveLovDetail(LovMaster model);
        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteLovDetail(string lovColumn, string? lovCode);
        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteLovMaster(string lovColumne);

    }
}
