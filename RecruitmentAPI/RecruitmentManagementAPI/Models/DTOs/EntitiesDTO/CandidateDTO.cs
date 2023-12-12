using System.ComponentModel.DataAnnotations;

namespace RecruitmentManagementAPI.Models.DTOs.EntitiesDTO
{
    public class CandidateDTO : UserDTO
    {
        [Required]
        public int RecruiterId { get; set; }
    }
}
