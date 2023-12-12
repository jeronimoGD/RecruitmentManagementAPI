using RecruitmentManagementAPI.Models.DTOs.EntitiesDTO;

namespace RecruitmentManagementAPI.Models.DTOs.LoginDTO
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
        public string Rol { get; set; }
    }
}
