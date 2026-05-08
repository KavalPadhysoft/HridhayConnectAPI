using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace HridhayConnect_API.Infra
{

    public class LogEntry
    {
        
        public static void InsertLogEntry(ErrorLog log)





        {
            if (log == null) throw new ArgumentNullException(nameof(log));

            SqlParameter[] spParams = new SqlParameter[]
            {
                new SqlParameter("@ApplicationName", SqlDbType.VarChar, 100) { Value = (object)log.ApplicationName ?? DBNull.Value },
                new SqlParameter("@ControllerName", SqlDbType.VarChar, 200) { Value = (object)log.ControllerName ?? DBNull.Value },
                new SqlParameter("@ErrorMessage", SqlDbType.VarChar, -1) { Value = (object)log.ErrorMessage ?? DBNull.Value },
                new SqlParameter("@ErrorType", SqlDbType.VarChar, 200) { Value = (object)log.ErrorType ?? DBNull.Value },
                new SqlParameter("@StackTrace", SqlDbType.VarChar, -1) { Value = (object)log.StackTrace ?? DBNull.Value },
                new SqlParameter("@RequestUrl", SqlDbType.VarChar, 500) { Value = (object)log.RequestUrl ?? DBNull.Value },
                new SqlParameter("@RequestPayload", SqlDbType.VarChar, -1) { Value = (object)log.RequestPayload ?? DBNull.Value },
                new SqlParameter("@UserAgent", SqlDbType.VarChar, 500) { Value = (object)log.UserAgent ?? DBNull.Value },
                new SqlParameter("@UserId", SqlDbType.BigInt) { Value = (object)log.UserId ?? DBNull.Value },
                new SqlParameter("@ClientIP", SqlDbType.VarChar, 50) { Value = (object)log.ClientIP ?? DBNull.Value },
                new SqlParameter("@CreatedBy", SqlDbType.VarChar, 100) { Value = (object)log.CreatedBy ?? DBNull.Value }
                
            };

            ExecuteSPForLogEntry("usp_ErrorLog_Insert", spParams);
        }

        
        public static void InsertLogEntryFromException(
            Exception ex,
            string applicationName = "padhyaso_vayda",
            string? controllerName = null,
            string? requestUrl = null,
            string? requestPayload = null,
            string? userAgent = null,
            long? userId = null,
            string? clientIP = null,
            string? createdBy = null)
        {
            if (ex == null) return;

            var log = new ErrorLog
            {
                ApplicationName = applicationName,
                ControllerName = controllerName,
                ErrorMessage = ex.Message,
                ErrorType = ex.GetType().FullName,
                StackTrace = ex.ToString(),
                RequestUrl = requestUrl,
                RequestPayload = requestPayload,
                UserAgent = userAgent,
                UserId = userId,
                ClientIP = clientIP,
                CreatedBy = createdBy
            };

            InsertLogEntry(log);
        }

        public static void ExecuteSPForLogEntry(string sp, SqlParameter[] spCol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppHttpContextAccessor.DataConnectionString))
                using (SqlCommand cmd = new SqlCommand(sp, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (spCol != null && spCol.Length > 0)
                    {
                        cmd.Parameters.AddRange(spCol);
                    }

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
              
                try
                {
                    System.Diagnostics.Trace.TraceError($"LogEntry.ExecuteSPForLogEntry failed for SP '{sp}': {ex}");
                }
                catch
                {
                    
                }
            }
        }
    }
 }
