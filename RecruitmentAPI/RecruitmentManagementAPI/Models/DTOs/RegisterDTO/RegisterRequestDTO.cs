using System.ComponentModel.DataAnnotations;

namespace RecruitmentManagementAPI.Models.DTOs.RegisterDTO
{
    public class RegisterRequestDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
