using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RecruitmentManagementAPI.Models.AplicationResponse;
using RecruitmentManagementAPI.Models.Constants;
using System.Text.RegularExpressions;

namespace RecruitmentManagementAPI.Services
{
    public class CommonUtils
    {
        private readonly APISettings _apiSettings;

        public CommonUtils(IOptions<APISettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }

        public ActionResult GetResult(ControllerBase controller, APIResponse response)
        {
            return response.Result == null && response.ErrorMessages.Count == 0 ? controller.StatusCode((int)response.StatusCode) : controller.StatusCode((int)response.StatusCode, response);
        }

        public bool IsStrongPassword(string password)
        {
            // Regular expression pattern to enforce password requirements
            // Default: Atleast 1 digit, 1 simbol, 1 uppercase, 1 lowercase and minimum length 8
            return Regex.IsMatch(password, _apiSettings.PasswordRequirements);
        }
    }
}
