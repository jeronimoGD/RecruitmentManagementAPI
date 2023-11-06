using Azure;
using Microsoft.AspNetCore.Mvc;
using RecruitmentManagementAPI.Models.AplicationResponse;

namespace RecruitmentManagementAPI.Services
{
    public class CommonUtils
    {
        public ActionResult GetResult(ControllerBase controller, APIResponse response)
        {
            return response.Result == null && response.ErrorMessages.Count == 0 ? controller.StatusCode((int)response.StatusCode) : controller.StatusCode((int)response.StatusCode, response);
        }
    }
}
