using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecruitmentManagementAPI.Data;
using RecruitmentManagementAPI.Models.Constants;
using RecruitmentManagementAPI.Models.DTOs.EntitiesDTO;
using RecruitmentManagementAPI.Models.DTOs.LoginDTO;
using RecruitmentManagementAPI.Models.Entities;
using RecruitmentManagementAPI.Services.Repository.IRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RecruitmentManagementAPI.Services.Repository
{
    public class CandidateRepository : Repository<Candidate>, ICandidateRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly APISettings _apiSettings;
        public CandidateRepository(ApplicationDbContext dbContext, IMapper mapper, IOptions<APISettings> apiSettings) : base(dbContext)
        {
            _apiSettings = apiSettings.Value;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public bool DoesUserExists(string userName)
        {
            var usuario = _dbContext.Recruiters.FirstOrDefault(user => user.Name.ToLower() == userName.ToLower());
            return usuario != null;
        }

        public async Task<LoginResponseDTO> LogIn(LoginRequestDTO loginRequestDTO)
        {
            var logedUser = await _dbContext.Candidates.FirstOrDefaultAsync(r => r.Name.ToLower() == loginRequestDTO.UserName.ToLower() &&
                                                                            r.Password == loginRequestDTO.Password);
            if (logedUser == null)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_apiSettings.IssuerSigningKey);
            var tokenDecsriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, logedUser.Id.ToString()),
                    new Claim(ClaimTypes.Role, logedUser.Rol)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };
            var token = tokenHandler.CreateToken(tokenDecsriptor);

            LoginResponseDTO loginResponse = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(logedUser)
            };

            return loginResponse;
        }
    }
}
