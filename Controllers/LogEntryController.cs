using HridhayConnect_API.Infra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace HridhayConnect_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LogEntryController : ControllerBase
    {


        private readonly IRepositoryBase<ErrorLog> _repository;

        public LogEntryController(IRepositoryBase<ErrorLog> repository)
        {
            _repository = repository;
        }

        [HttpPost("[Action]")]
        public IActionResult save([FromBody] ErrorLog dto)
        {


            if (dto == null) return BadRequest("Payload is required.");

            // Map DTO to your existing ErrorLog (make sure this class exists in your project)
            var log = new ErrorLog
            {
                ApplicationName = dto.ApplicationName,
                ControllerName = dto.ControllerName,
                ErrorMessage = dto.ErrorMessage,
                ErrorType = dto.ErrorType,
                StackTrace = dto.StackTrace,
                RequestUrl = dto.RequestUrl,
                RequestPayload = dto.RequestPayload,
                UserAgent = dto.UserAgent,
                UserId = dto.UserId,
                ClientIP = dto.ClientIP,
                CreatedBy = dto.CreatedBy
            };

            try
            {
                LogEntry.InsertLogEntry(log);
                return StatusCode(201); // Created
            }
            catch (System.Exception ex)
            {
                // Avoid throwing another exception into the logging path — return 500
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet("[Action]")]
        public IActionResult GetPaged( int start = 0, int length = 10, string sortColumn = "", string sortColumnDir = "asc", string searchValue = "")
        {
            try
            {
                SqlParameter[] parameters = null; // No SP parameters

                var result = _repository.ExecuteWithPagination(
                    "SP_ErroLog_Get",
                    parameters,
                    start,
                    length,
                    sortColumn,
                    sortColumnDir,
                    searchValue
                );

                return Ok(new
                {
                    success = true,
                    message = "Error logs fetched successfully",
                    data = result.Data,
                    recordsTotal = result.RecordsTotal,
                    recordsFiltered = result.RecordsFiltered
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }



    }
}
