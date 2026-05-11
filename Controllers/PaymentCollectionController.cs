using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.PaymentCollectionRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;


namespace HridhayConnect_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentCollectionController : ControllerBase
    {
        private readonly IPaymentCollectionRepository _paymentCollectionRepository  ;
        private readonly CommonViewModel CommonViewModel = new();
        private readonly ValidationService _validation;

        public PaymentCollectionController(IPaymentCollectionRepository paymentCollectionRepository, ValidationService validation)
        {
            _paymentCollectionRepository = paymentCollectionRepository;
            _validation = validation;
        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> GetAllPaymentCollections(
            int start = 0, int length = 10,
            string sortColumn = "", string sortColumnDir = "asc",
            string searchValue = "")
        {
            try
            {
                var data = await _paymentCollectionRepository.GetAllPaymentCollections(
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
        public async Task<IActionResult> Add(PaymentCollection paymentCollection)
        {
            try
            {
                if (paymentCollection.CustomerId == null || paymentCollection.CustomerId <= 0)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please Select Shop Name";
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    return Ok(CommonViewModel);
                }
                if (paymentCollection.PaymentDate == null)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please Select Payment Date";
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    return Ok(CommonViewModel);
                }
                if (paymentCollection.Amount == null || paymentCollection.Amount <= 0)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please Enter Amount";
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    return Ok(CommonViewModel);
                }
                if (string.IsNullOrWhiteSpace(paymentCollection.PaymentMode))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please Select Payment Mode";
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    return Ok(CommonViewModel);
                }
                var (IsSuccess, Message, Id, Extra) = await _paymentCollectionRepository.AddOrUpdatePaymentCollection(paymentCollection);
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
                var data = await _paymentCollectionRepository.GetPaymentCollectionById(id);

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
        [HttpGet("[Action]")]
        public async Task<IActionResult> GetAllOutstandingPayments(
           int start = 0, int length = 10,
           string sortColumn = "", string sortColumnDir = "asc",
           string searchValue = "")
        {
            try
            {
                var data = await _paymentCollectionRepository.GetAllOutstandingPayments(
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
        //[HttpDelete("[Action]")]
        //public async Task<IActionResult> Delete(long id)
        //{
        //    try
        //    {
        //        var (IsSuccess, Message, Id, Extra) = await _customersRepository.DeleteByCustomerId(id);
        //        CommonViewModel.IsSuccess = IsSuccess;
        //        CommonViewModel.IsConfirm = true;
        //        CommonViewModel.StatusCode = IsSuccess ? ResponseStatusCode.Success : ResponseStatusCode.Error;
        //        CommonViewModel.Message = Message;
        //        CommonViewModel.Data = Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonViewModel.IsSuccess = false;
        //        CommonViewModel.StatusCode = ResponseStatusCode.Error;
        //        CommonViewModel.Message = ex.Message;
        //    }

        //    return Ok(CommonViewModel);
        //}
    }

}

