using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentManagementAPI.Models;
using RecruitmentManagementAPI.Models.AplicationResponse;
using RecruitmentManagementAPI.Models.DTOs.EntitiesDTO;
using RecruitmentManagementAPI.Models.DTOs.RegisterDTO;
using RecruitmentManagementAPI.Models.Entities;
using RecruitmentManagementAPI.Services;
using RecruitmentManagementAPI.Services.Repository.IRepository;
using System.Net;

namespace RecruitmentManagementAPI.Controllers
{
    [Route("api/RecruiterControler")]
    [ApiController]
    public class RecruiterController : ControllerBase
    {
        private readonly ILogger<RecruiterController> _logger;
        protected APIResponse _response;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly CommonUtils _commonUtils;

        public RecruiterController(ILogger<RecruiterController> logger,IUnitOfWork unitOfWork, IMapper mapper, CommonUtils commonUtils)
        {
            _response = new APIResponse();
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonUtils = commonUtils;
        }

        [HttpGet(Name = "GetRecruiters")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetRecruiters()
        {
            try
            {
                IEnumerable<Recruiter> recruiters = await _unitOfWork.Recruiters.GetAll();

                if (recruiters == null)
                {
                    _response = APIResponse.NotFound(new List<string> { "No recruiters found" });
                }
                else
                {
                    _response = APIResponse.Ok(_mapper.Map<IEnumerable<UserDTO>>(recruiters));
                }
            }
            catch (Exception e)
            {
                _response = APIResponse.InternalServerError(new List<string>() { e.ToString() });
            }

            return _commonUtils.GetResult(this, _response);
        }

        [HttpGet("id:int", Name = "GetRecruiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetRecruiter(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response = APIResponse.BadRequest(null, new List<string> { $"Introduced id {id} is not valid. I has to be greater than 0." });
                }
                else
                {
                    var recruiter = await _unitOfWork.Recruiters.Get(v => v.Id == id);

                    if (recruiter == null)
                    {
                        _response = APIResponse.NotFound(new List<string> { $"Recruiter with id {id} not found" });
                    }
                    else
                    {
                        _response = APIResponse.Ok(_mapper.Map<UserDTO>(recruiter));
                    }
                }
            }
            catch (Exception e)
            {
                _response = APIResponse.InternalServerError(new List<string>() { e.ToString() });
            }

            return _commonUtils.GetResult(this, _response);
        }

        [HttpPost ( Name = "CreateRecruiter") ]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateRecruiter([FromBody] UserCreateDTO recruiterCreateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response = APIResponse.BadRequest(ModelState, new List<string> { $"Wrong input model {recruiterCreateDTO}" });
                }
                else
                {
                    var doesRecruiterExist = await _unitOfWork.Recruiters.Get(v => v.Name.ToLower() == recruiterCreateDTO.Name.ToLower());
                    
                    if (doesRecruiterExist != null)
                    {
                        _response = APIResponse.BadRequest(ModelState, new List<string> { $"The recruiter with name {recruiterCreateDTO.Name} already exists" });
                    }
                    else
                    {

                        Recruiter newRecruiter = _mapper.Map<Recruiter>(recruiterCreateDTO);
                        newRecruiter.CreationTime = DateTime.UtcNow;
                        newRecruiter.UpdateTime = DateTime.UtcNow;
                        newRecruiter.Rol = "recruiter";

                        await _unitOfWork.Recruiters.Create(newRecruiter);

                        _response = APIResponse.Created(newRecruiter);
                    }
                }               
            }
            catch (Exception e)
            {
                _response = APIResponse.InternalServerError(new List<string>() { e.ToString() });
            }

            return _commonUtils.GetResult(this, _response);
        }

        [HttpDelete("{id:int}", Name = "DeleteRecruiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRecruiter(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response = APIResponse.BadRequest(null, new List<string> { $"Introduced id {id} is not valid. I has to be greater than 0." });
                }
                else
                {
                    var recruiter = await _unitOfWork.Recruiters.Get(v => v.Id == id);

                    if (recruiter == null)
                    {
                        _response = APIResponse.NotFound(new List<string> { $"Recruiter with id {id} not found" });
                    }
                    else
                    {
                        await _unitOfWork.Recruiters.Delete(recruiter);
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

        [HttpPut("{id:int}", Name = "UpdateRecruiter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRecruiter(int id,[FromBody] UserUpdateDTO recruiterUpdateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response = APIResponse.BadRequest(ModelState, new List<string> { $"Wrong input model {recruiterUpdateDTO}" });
                }
                else if (id != recruiterUpdateDTO.Id || id <= 0)
                {
                    _response = APIResponse.BadRequest(ModelState, new List<string> { $"Introduced id {id} is not valid. I has to be greater than 0 and equal to the model id." });
                }
                else
                {
                    var doesRecruiterExist = await _unitOfWork.Recruiters.Get(v => v.Id == recruiterUpdateDTO.Id, false);
                    if (doesRecruiterExist == null)
                    {
                        _response = APIResponse.BadRequest(ModelState, new List<string> { $"The recruiter with id {recruiterUpdateDTO.Id} does not exists" });
                    }
                    else
                    {
                        Recruiter updatedRecruiter = _mapper.Map<Recruiter>(recruiterUpdateDTO);
                        updatedRecruiter.UpdateTime = DateTime.UtcNow;
                        updatedRecruiter.CreationTime = DateTime.UtcNow;
                        updatedRecruiter.Rol = "recruiter";

                        await _unitOfWork.Recruiters.Update(updatedRecruiter);
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
