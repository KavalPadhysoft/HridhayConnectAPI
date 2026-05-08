using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using System.Data;

namespace HridhayConnect_API.ServiceRepository.LovRepository
{
    public class LovRepository : ILovRepository
    {
        private readonly IRepositoryBase<LovMaster> _repositoryBase;
        private readonly DataContext _context;

        public LovRepository(DataContext context, IRepositoryBase<LovMaster> repositoryBase)
        {
            _repositoryBase = repositoryBase;
            _context = context;
        }

        // =====================================
        // SP 1: SP_LOV_Get
        // =====================================
        public async Task<List<LovMaster>> GetLov(
     string lovColumn,
     string lovCode,
     string flag)
        {
            List<LovMaster> list = new();

            using (SqlConnection con =
                   new SqlConnection(_context.Database.GetConnectionString()))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SP_LOV_Get", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@Lov_Column", SqlDbType.VarChar)
                    {
                        Value = lovColumn ?? ""
                    });

                    cmd.Parameters.Add(new SqlParameter("@Lov_Code", SqlDbType.VarChar)
                    {
                        Value = lovCode ?? ""
                    });

                    cmd.Parameters.Add(new SqlParameter("@Flag", SqlDbType.VarChar)
                    {
                        Value = flag
                    });

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new();
                        dt.Load(reader);

                        if (dt.Rows.Count > 0)
                        {
                            string json = JsonConvert.SerializeObject(dt);
                            list = JsonConvert.DeserializeObject<List<LovMaster>>(json)
                                   ?? new List<LovMaster>();
                        }
                    }
                }
            }

            return await Task.FromResult(list);
        }


        // =====================================
        // SP 2: SP_Lov_Save (MASTER)
        // =====================================
        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveLovMaster(LovMaster model)
        {
            try
            {
                //     model.Action = string.IsNullOrWhiteSpace(model.Lov_Code)
                //? "INSERT"
                //: "UPDATE";
                List<SqlParameter> oParams = new();


                oParams.Add(new SqlParameter("@Lov_Column", model.Lov_Column));
                oParams.Add(new SqlParameter("@Display_Text", model.Display_Text));
                oParams.Add(new SqlParameter("@Operated_By", AppHttpContextAccessor.JwtUserId));
                oParams.Add(new SqlParameter("@Action", model.Action));


                var result = _repositoryBase.ExecuteStoredProcedurenew("SP_Lov_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        // =====================================
        // SP 3: SP_LovDtl_Save
        // =====================================
        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveLovDetail(LovMaster model)
        {
            try
            {
                model.Action = string.IsNullOrWhiteSpace(model.Lov_Code)
           ? "INSERT"
           : "UPDATE";
                List<SqlParameter> oParams = new();


                oParams.Add(new SqlParameter("@Lov_Column", model.Lov_Column));
                oParams.Add(new SqlParameter("@Display_Text", model.Display_Text));
                oParams.Add(new SqlParameter("@Lov_Code", model.Lov_Code ?? ""));
                oParams.Add(new SqlParameter("@Lov_Desc", model.Lov_Desc));
                oParams.Add(new SqlParameter("@DisplayOrder", model.DisplayOrder));
                oParams.Add(new SqlParameter("@IsActive", model.IsActive));
                oParams.Add(new SqlParameter("@Operated_By", model.CreatedBy));
                oParams.Add(new SqlParameter("@Action", model.Action));


                var result = _repositoryBase.ExecuteStoredProcedurenew("SP_LovDtl_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // =====================================
        // SP 4: SP_LovDtl_Delete
        // =====================================
        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteLovDetail(
            string lovColumn,
            string? lovCode)
        {

            try
            {
                List<SqlParameter> parameters = new()
                {
                new SqlParameter("@Lov_Column", lovColumn),
                new SqlParameter("@Lov_Code", lovCode),
                new SqlParameter("@Operated_By", AppHttpContextAccessor.JwtUserId)
                };

                var result = _repositoryBase.ExecuteStoredProcedurenew("SP_LovDtl_Delete", parameters, true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteLovMaster(
    string lovColumn)
        {

            try
            {
                List<SqlParameter> parameters = new()
                {
                new SqlParameter("@Lov_Column", lovColumn),
                new SqlParameter("@Operated_By", AppHttpContextAccessor.JwtUserId)
                };

                var result = _repositoryBase.ExecuteStoredProcedurenew("SP_LovMaster_Delete", parameters, true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
