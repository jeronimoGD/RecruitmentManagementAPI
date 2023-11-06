using RecruitmentManagementAPI.Models.DTOs.EntitiesDTO;
using RecruitmentManagementAPI.Models.DTOs.LoginDTO;
using RecruitmentManagementAPI.Models.DTOs.RegisterDTO;
using RecruitmentManagementAPI.Models.Entities;

namespace RecruitmentManagementAPI.Services.Repository.IRepository
{
    public interface IRecruiterRepository : IRepository<Recruiter>
    {
        bool DoesUserExists(string userName);
        Task<LoginResponseDTO> LogIn(LoginRequestDTO loginRequestDTO);
        Task<Recruiter> Register(RegisterRequestDTO registerRequestDTO);
    }
}
