using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecruitmentManagementAPI.Data;
using RecruitmentManagementAPI.Models.AplicationConstants;
using RecruitmentManagementAPI.Models.Constants;
using RecruitmentManagementAPI.Models.DTOs.EntitiesDTO;
using RecruitmentManagementAPI.Models.DTOs.LoginDTO;
using RecruitmentManagementAPI.Models.DTOs.RegisterDTO;
using RecruitmentManagementAPI.Models.Entities;
using RecruitmentManagementAPI.Services.Repository.IRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RecruitmentManagementAPI.Services.Repository
{
    public class RecruiterRepository : Repository<Recruiter>, IRecruiterRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly APISettings _apiSettings;
        private readonly CommonUtils _commonUtils;

        public RecruiterRepository(ApplicationDbContext dbContext, IMapper mapper, IOptions<APISettings> apiSettings, CommonUtils commonUtils) : base(dbContext)
        {
            _apiSettings = apiSettings.Value;
            _dbContext = dbContext;
            _mapper = mapper;
            _commonUtils = commonUtils;
        }

        public bool DoesUserExists(string userName)
        {
            var usuario = _dbContext.Recruiters.FirstOrDefault(user => user.Name.ToLower() == userName.ToLower());
            return usuario != null;
        }

        public async Task<LoginResponseDTO> LogIn(LoginRequestDTO loginRequestDTO)
        {
            LoginResponseDTO loginResponse = new LoginResponseDTO()
            {
                Token = "",
                User = null
            };

            var logedUser = await _dbContext.Recruiters.FirstOrDefaultAsync(r => r.Name.ToLower() == loginRequestDTO.UserName.ToLower() &&
                                                                     r.Password == _commonUtils.HashPasswordSHA256(loginRequestDTO.Password));
            if (logedUser != null)
            {

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

                loginResponse = new LoginResponseDTO()
                {
                    Token = tokenHandler.WriteToken(token),
                    User = _mapper.Map<UserDTO>(logedUser)
                };
            }

            return loginResponse;
        }

        public async Task<Recruiter> Register(RegisterRequestDTO registerRequestDTO)
        {
            Recruiter newRecruiter = new Recruiter()
            {
                Name = registerRequestDTO.Name,
                Email = registerRequestDTO.Email,
                Password = _commonUtils.HashPasswordSHA256(registerRequestDTO.Password),
                Rol = APIConstants.RecruiterRole,
                CreationTime = DateTime.UtcNow,
                UpdateTime = DateTime.UtcNow
            };

            await _dbContext.Recruiters.AddAsync(newRecruiter);
            await _dbContext.SaveChangesAsync();   
            return newRecruiter;
        }
    }
}
