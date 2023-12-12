using RecruitmentManagementAPI.Models.DTOs.LoginDTO;
using RecruitmentManagementAPI.Models.Entities;

namespace RecruitmentManagementAPI.Services.Repository.IRepository
{
    public interface ICandidateRepository : IRepository<Candidate>
    {
        bool DoesUserExists(string userName);
        Task<LoginResponseDTO> LogIn(LoginRequestDTO loginRequestDTO);
    }
}
