using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.CustomersRepository;
using HridhayConnect_API.ServiceRepository.MenuRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;


namespace HridhayConnect_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomersRepository _customersRepository;
        private readonly CommonViewModel CommonViewModel = new();
        private readonly ValidationService _validation;

        public CustomersController(ICustomersRepository customersRepository, ValidationService validation)
        {
            _customersRepository = customersRepository;
            _validation = validation;
        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> GetAllpage(
            int start = 0, int length = 10,
            string sortColumn = "", string sortColumnDir = "asc",
            string searchValue = "")
        {
            try
            {
                var data = await _customersRepository.GetAllCustomers(
                    start, length, sortColumn, sortColumnDir, searchValue);

                CommonViewModel.IsSuccess = true;
                CommonViewModel.StatusCode = ResponseStatusCode.Success;
                CommonViewModel.Data = data;
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
        public async Task<IActionResult> Add(Customers customers)
        {
            try
            {
                var Name = _validation.ValidateRequired(customers.Name, "Shop Name"); 
                if (!Name.IsSuccess) { return Ok(Name); }
                var OwnerName = _validation.ValidateRequired(customers.OwnerName, "Owner Name");
                if (!OwnerName.IsSuccess) { return Ok(OwnerName); }
                var Phone = _validation.ValidateRequired(customers.Phone, "Phone No");
                if (!Phone.IsSuccess) { return Ok(Phone); }
                var mobileValidation = _validation.ValidateMobile(customers.Phone);
                if (!mobileValidation.IsSuccess) { return Ok(mobileValidation); }
                if (!string.IsNullOrWhiteSpace(customers.Email))
                {
                    var email = _validation.ValidateEmail(customers.Email);
                    if (!email.IsSuccess)
                    {
                        return Ok(email);
                    }
                }
                if (!string.IsNullOrWhiteSpace(customers.Pincode.ToString()))
                {
                    var pincode = _validation.ValidatePincode(customers.Pincode.ToString());
                    if (!pincode.IsSuccess)
                    {
                        return Ok(pincode);
                    }
                }
                var (IsSuccess, Message, Id, Extra) = await _customersRepository.AddOrUpdateCustomers(customers);
                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.IsConfirm = true;
                CommonViewModel.StatusCode = IsSuccess ? ResponseStatusCode.Success : ResponseStatusCode.Error;
                CommonViewModel.Message = Message;
                CommonViewModel.Data = Id;
            }
            catch (Exception ex)
            {
                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;
            }

            return Ok(CommonViewModel);


        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var data = await _customersRepository.GetCustomerById(id);

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
            }

            return Ok(CommonViewModel);
        }

        [HttpDelete("[Action]")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var (IsSuccess, Message, Id, Extra) = await _customersRepository.DeleteByCustomerId(id);
                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.IsConfirm = true;
                CommonViewModel.StatusCode = IsSuccess ? ResponseStatusCode.Success : ResponseStatusCode.Error;
                CommonViewModel.Message = Message;
                CommonViewModel.Data = Id;
            }
            catch (Exception ex)
            {
                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;
            }

            return Ok(CommonViewModel);
        }
    }

}

