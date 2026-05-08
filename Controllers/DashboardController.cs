using HridhayConnect_API.Infra;
using HridhayConnect_API.Models;
using HridhayConnect_API.ServiceRepository.DashboardRepository;
using HridhayConnect_API.ServiceRepository.ItemRepository;
using HridhayConnect_API.ServiceRepository.MenuRepository;
using HridhayConnect_API.ServiceRepository.OrderRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace HridhayConnect_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly CommonViewModel CommonViewModel = new();
        private readonly ValidationService _validation;

        public DashboardController(IDashboardRepository dashboardRepository , ValidationService validation)
        {
            _dashboardRepository = dashboardRepository;
            _validation = validation;
        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> GetDashboardDataForPendingOrders(
            int start = 0, int length = 10,
            string sortColumn = "", string sortColumnDir = "asc",
            string searchValue = "")
        {
            try
            {
                var data = await _dashboardRepository.GetDashboardDataForPendingOrders(
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




        [HttpGet("[Action]")]
        public async Task<IActionResult> GetDashboardDataForPendingItemWise(
            int start = 0, int length = 10,
            string sortColumn = "", string sortColumnDir = "asc",
            string searchValue = "")
        {
            try
            {
                var data = await _dashboardRepository.GetDashboardDataForPendingItemWise(
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


    }

}

