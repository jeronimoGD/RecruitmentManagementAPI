using Azure;
using Microsoft.AspNetCore.Mvc;
using RecruitmentManagementAPI.Models.AplicationResponse;
using System.Text.RegularExpressions;

namespace RecruitmentManagementAPI.Services
{
    public class CommonUtils
    {
        public ActionResult GetResult(ControllerBase controller, APIResponse response)
        {
            return response.Result == null && response.ErrorMessages.Count == 0 ? controller.StatusCode((int)response.StatusCode) : controller.StatusCode((int)response.StatusCode, response);
        }

        public bool IsStrongPassword(string password)
        {
            // Regular expression pattern to enforce password requirements
            // Atleast 1 digit, 1 simbol, 1 uppercase, 1 lowercase and minimum length 8
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]).{8,}$";

            // Check if the password matches the pattern
            return Regex.IsMatch(password, pattern);
        }
    }
}
