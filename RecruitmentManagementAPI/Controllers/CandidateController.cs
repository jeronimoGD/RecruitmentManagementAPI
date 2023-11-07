using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentManagementAPI.Models.AplicationConstants;
using RecruitmentManagementAPI.Models.AplicationResponse;
using RecruitmentManagementAPI.Models.Constants;
using RecruitmentManagementAPI.Models.DTOs.EntitiesDTO;
using RecruitmentManagementAPI.Models.DTOs.LoginDTO;
using RecruitmentManagementAPI.Models.Entities;
using RecruitmentManagementAPI.Services;
using RecruitmentManagementAPI.Services.Repository.IRepository;
using System.Security.Claims;

namespace RecruitmentManagementAPI.Controllers
{
    [Route("api/CandidateController")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        private readonly ILogger<CandidateController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly CommonUtils _commonUtils;
        protected APIResponse _response;

        public CandidateController(ILogger<CandidateController> logger, IUnitOfWork unitOfWork, IMapper mapper, CommonUtils commonUtils)
        {
            _response = new APIResponse();
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonUtils = commonUtils;
        }

        [HttpGet("GetCandidates", Name = "GetCandidates")]
        [Authorize(Roles = APIConstants.RecruiterRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetCandidates()
        {
            try
            {
                IEnumerable<Candidate> candidates = await _unitOfWork.Candidates.GetAll();

                if (candidates == null)
                {
                    _response = APIResponse.NotFound(new List<string> { "No candidates found" });
                }
                else
                {
                    _response = APIResponse.Ok(_mapper.Map<IEnumerable<CandidateDTO>>(candidates));
                }
            }
            catch (Exception e)
            {
                _response = APIResponse.InternalServerError(new List<string>() { e.ToString() });
            }

            return _commonUtils.GetResult(this, _response);
        }

        [HttpGet("GetCreatedCandidates", Name = "GetCreatedCandidates")]
        [Authorize(Roles = APIConstants.RecruiterRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetCreatedCandidates()
        {
            try
            {
                var recriterCreatorID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                IEnumerable<Candidate> candidates = await _unitOfWork.Candidates.GetAll(c => c.RecruiterId == int.Parse(recriterCreatorID));

                if (candidates == null)
                {
                    _response = APIResponse.NotFound(new List<string> { "No candidates found" });
                }
                else
                {
                    _response = APIResponse.Ok(_mapper.Map<IEnumerable<CandidateDTO>>(candidates));
                }
            }
            catch (Exception e)
            {
                _response = APIResponse.InternalServerError(new List<string>() { e.ToString() });
            }

            return _commonUtils.GetResult(this, _response);
        }

        [HttpPost("CreateCandidate", Name = "CreateCandidate")]
        [Authorize(Roles = APIConstants.RecruiterRole)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<APIResponse>> CreateCandidate([FromBody] CandidateCreateDTO candidateCreateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response = APIResponse.BadRequest(ModelState, new List<string> { $"Wrong input model {candidateCreateDTO}" });
                }
                else
                {
                    var doesCandidateExist = await _unitOfWork.Candidates.Get(v => v.Name.ToLower() == candidateCreateDTO.Name.ToLower());

                    if (doesCandidateExist != null)
                    {
                        _response = APIResponse.BadRequest(ModelState, new List<string> { $"The candidate with name {candidateCreateDTO.Name} already exists" });
                    }
                    else
                    {
                        if (!_commonUtils.IsStrongPassword(candidateCreateDTO.Password))
                        {
                            _response = APIResponse.BadRequest(ModelState, new List<string> { $"The password is no strong enough. Please create a minimum 8 characters password with atleast 1 digit, 1 simbol, 1 uppercase and 1 lowercase" });
                        }
                        else
                        {
                            var recriterCreatorID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                            Candidate newCandidate = _mapper.Map<Candidate>(candidateCreateDTO);
                            newCandidate.CreationTime = DateTime.UtcNow;
                            newCandidate.UpdateTime = DateTime.UtcNow;
                            newCandidate.Rol = APIConstants.CandidateRole;
                            newCandidate.RecruiterId = int.Parse(recriterCreatorID);

                            await _unitOfWork.Candidates.Create(newCandidate);
                            _response = APIResponse.Created(newCandidate);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _response = APIResponse.InternalServerError(new List<string>() { e.ToString() });
            }

            return _commonUtils.GetResult(this, _response);
        }

        [HttpPost("CandidateLogIn", Name = "CandidateLogIn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CandidateLogIn([FromBody] LoginRequestDTO loginRequestDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response = APIResponse.BadRequest(ModelState, new List<string> { $"Wrong input model {loginRequestDTO}" });
                }
                else
                {
                    var loginResponse = await _unitOfWork.Candidates.LogIn(loginRequestDTO);

                    if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
                    {
                        _response = APIResponse.BadRequest(ModelState, new List<string> { $"Wrong UserName ({loginRequestDTO.UserName}) or Password ({loginRequestDTO.Password})" });
                    }
                    else
                    {
                        _response = APIResponse.Ok(loginResponse);
                    }
                }
            }
            catch (Exception e)
            {
                _response = APIResponse.InternalServerError(new List<string>() { e.ToString() });
            }

            return _commonUtils.GetResult(this, _response);
        }

        [HttpDelete("DeleteCandidate", Name = "DeleteCandidate")]
        [Authorize(Roles = APIConstants.RecruiterRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCandidate(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response = APIResponse.BadRequest(null, new List<string> { $"Introduced id {id} is not valid. I has to be greater than 0." });
                }
                else
                {
                    var candidateToDelete = await _unitOfWork.Candidates.Get(v => v.Id == id);
                    var recriterCreatorID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (candidateToDelete == null)
                    {
                        _response = APIResponse.NotFound(new List<string> { $"Candidate with id {id} not found" });
                    }
                    else if (candidateToDelete.RecruiterId != int.Parse(recriterCreatorID))
                    {
                        _response = APIResponse.BadRequest(ModelState, new List<string> { $"You can not delete this candidate due to you are not the creator." });
                    }
                    else
                    {
                        await _unitOfWork.Candidates.Delete(candidateToDelete);
                        _response = APIResponse.NoContent();
                    }
                }

            }
            catch (Exception e)
            {
                _response = APIResponse.InternalServerError(new List<string>() { e.ToString() });
            }

            return _commonUtils.GetResult(this, _response);
        }

        [HttpPut("UpdateCandidate", Name = "UpdateCandidate")]
        [Authorize(Roles = APIConstants.RecruiterRole)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCandidate(int id, [FromBody] UserUpdateDTO candidateUpdateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response = APIResponse.BadRequest(ModelState, new List<string> { $"Wrong input model {candidateUpdateDTO}" });
                }
                else if (id != candidateUpdateDTO.Id || id <= 0)
                {
                    _response = APIResponse.BadRequest(ModelState, new List<string> { $"Introduced id {id} is not valid. I has to be greater than 0 and equal to the model id." });
                }
                else
                {
                    var doesCandidateExist = await _unitOfWork.Candidates.Get(v => v.Id == candidateUpdateDTO.Id, false);
                    var recriterCreatorID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (doesCandidateExist == null)
                    {
                        _response = APIResponse.BadRequest(ModelState, new List<string> { $"The candidate with id {candidateUpdateDTO.Id} does not exists" });
                    }
                    else if (doesCandidateExist.RecruiterId != int.Parse(recriterCreatorID))
                    {
                        _response = APIResponse.BadRequest(ModelState, new List<string> { $"You can not modify this candidate due to you are not the creator." });
                    }
                    else
                    {
                        if (!_commonUtils.IsStrongPassword(candidateUpdateDTO.Password))
                        {
                            _response = APIResponse.BadRequest(ModelState, new List<string> { $"The password is no strong enough. Please create a minimum 8 characters password with atleast 1 digit, 1 simbol, 1 uppercase and 1 lowercase" });
                        }
                        else
                        {
                            Candidate updatedCandidate = _mapper.Map<Candidate>(candidateUpdateDTO);
                            updatedCandidate.UpdateTime = DateTime.UtcNow;
                            updatedCandidate.CreationTime = DateTime.UtcNow;
                            updatedCandidate.Rol = APIConstants.CandidateRole;
                            updatedCandidate.RecruiterId = int.Parse(recriterCreatorID);

                            await _unitOfWork.Candidates.Update(updatedCandidate);
                            _response = APIResponse.NoContent();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _response = APIResponse.InternalServerError(new List<string>() { e.ToString() });
            }

            return _commonUtils.GetResult(this, _response);
        }
    }
}
