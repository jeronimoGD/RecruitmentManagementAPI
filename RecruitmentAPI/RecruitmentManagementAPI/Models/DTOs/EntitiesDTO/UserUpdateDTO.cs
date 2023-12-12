using System.ComponentModel.DataAnnotations;

namespace RecruitmentManagementAPI.Models.DTOs.EntitiesDTO
{
    public class UserUpdateDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
