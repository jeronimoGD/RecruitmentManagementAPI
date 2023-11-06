using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentManagementAPI.Models.AplicationResponse;
using RecruitmentManagementAPI.Models.DTOs.EntitiesDTO;
using RecruitmentManagementAPI.Models.Entities;
using RecruitmentManagementAPI.Services;
using RecruitmentManagementAPI.Services.Repository.IRepository;

namespace RecruitmentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        private readonly ILogger<CandidateController> _logger;
        protected APIResponse _response;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly CommonUtils _commonUtils;

        public CandidateController(ILogger<CandidateController> logger, IUnitOfWork unitOfWork, IMapper mapper, CommonUtils commonUtils)
        {
            _response = new APIResponse();
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonUtils = commonUtils;
        }

        [HttpGet(Name = "GetCandidates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        [HttpGet("id:int", Name = "GetCandidate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetCandidate(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response = APIResponse.BadRequest(null, new List<string> { $"Introduced id {id} is not valid. I has to be greater than 0." });
                }
                else
                {
                    var candidate = await _unitOfWork.Candidates.Get(v => v.Id == id);

                    if (candidate == null)
                    {
                        _response = APIResponse.NotFound(new List<string> { $"Candidate with id {id} not found" });
                    }
                    else
                    {
                        _response = APIResponse.Ok(_mapper.Map<CandidateDTO>(candidate));
                    }
                }
            }
            catch (Exception e)
            {
                _response = APIResponse.InternalServerError(new List<string>() { e.ToString() });
            }

            return _commonUtils.GetResult(this, _response);
        }

        [HttpPost(Name = "CeateCandidate")]
        [ProducesResponseType(StatusCodes.Status201Created)]
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

                        Candidate newCandidate = _mapper.Map<Candidate>(candidateCreateDTO);
                        newCandidate.CreationTime = DateTime.UtcNow;
                        newCandidate.UpdateTime = DateTime.UtcNow;
                        newCandidate.Rol = "candidate";

                        await _unitOfWork.Candidates.Create(newCandidate);
                        _response = APIResponse.Created(newCandidate);
                    }
                }
            }
            catch (Exception e)
            {
                _response = APIResponse.InternalServerError(new List<string>() { e.ToString() });
            }

            return _commonUtils.GetResult(this, _response);
        }

        [HttpDelete("{id:int}", Name = "DeleteCandidate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                    var candidate = await _unitOfWork.Candidates.Get(v => v.Id == id);

                    if (candidate == null)
                    {
                        _response = APIResponse.NotFound(new List<string> { $"Candidate with id {id} not found" });
                    }
                    else
                    {
                        await _unitOfWork.Candidates.Delete(candidate);
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

        [HttpPut("{id:int}", Name = "UpdateCandidate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                    if (doesCandidateExist == null)
                    {
                        _response = APIResponse.BadRequest(ModelState, new List<string> { $"The candidate with id {candidateUpdateDTO.Id} does not exists" });
                    }
                    else
                    {
                        Candidate updatedCandidate = _mapper.Map<Candidate>(candidateUpdateDTO);
                        updatedCandidate.UpdateTime = DateTime.UtcNow;
                        updatedCandidate.CreationTime = DateTime.UtcNow;
                        updatedCandidate.Rol = "candidate";

                        await _unitOfWork.Candidates.Update(updatedCandidate);
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
    }
}
