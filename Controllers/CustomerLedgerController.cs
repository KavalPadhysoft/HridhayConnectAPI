using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.CategoryRepository;
using HridhayConnect_API.ServiceRepository.CustomerLedgerRepository;
using HridhayConnect_API.ServiceRepository.MenuRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace HridhayConnect_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerLedgerController : ControllerBase
    {
        private readonly ICustomerLedgerRepository _customerLedgerRepository;
        private readonly CommonViewModel CommonViewModel = new();
        private readonly ValidationService _validation;
             

        public CustomerLedgerController(ICustomerLedgerRepository customerLedgerRepository, ValidationService validation)
        {
            _customerLedgerRepository = customerLedgerRepository;
            _validation = validation;
        }

        [HttpGet("[Action]")]
        [AllowAnonymous]
        public async Task<IActionResult> CustomerLedger(int start = 0, int length = 10, string sortColumn = "", string sortColumnDir = "asc", string searchValue = "",
  
  long? customerId = null
  )
        {
            var data = await _customerLedgerRepository.CustomerLedger(start, length, sortColumn, sortColumnDir, searchValue, customerId);

            CommonViewModel.IsSuccess = true;
            CommonViewModel.StatusCode = ResponseStatusCode.Success;
            CommonViewModel.Data = data;

            return Ok(CommonViewModel);
        }



    }

}

