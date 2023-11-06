using System.ComponentModel.DataAnnotations;

namespace RecruitmentManagementAPI.Models.DTOs.EntitiesDTO
{
    public class CandidateCreateDTO : UserCreateDTO 
    {
        [Required]
        public int RecruiterId { get; set; }
    }
}
