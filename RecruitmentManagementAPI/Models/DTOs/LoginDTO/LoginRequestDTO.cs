using System.ComponentModel.DataAnnotations;

namespace RecruitmentManagementAPI.Models.DTOs.LoginDTO
{
    public class LoginRequestDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
