using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentManagementAPI.Models.AplicationConstants;
using RecruitmentManagementAPI.Models.AplicationResponse;
using RecruitmentManagementAPI.Models.DTOs.EntitiesDTO;
using RecruitmentManagementAPI.Models.Entities;
using RecruitmentManagementAPI.Services.Repository.IRepository;
using RecruitmentManagementAPI.Services;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Azure;
using System.Xml.Linq;
using System.Collections.Generic;

namespace RecruitmentManagementAPI.Controllers
{
    [Route("api/DocumentControler")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        /*
         {
          "fileName": "prueba",
          "csvColumns": [
            "Name",
            "Email"
          ],
          "csvData": [
            [
              "Jero","jero@gmailcom"
            ],
            [
              "Juan","juan@gmailcom"
            ]
          ]
        }*/

        private readonly ILogger<RecruiterController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly CommonUtils _commonUtils;
        protected APIResponse _response;

        public DocumentController(ILogger<RecruiterController> logger, IUnitOfWork unitOfWork, IMapper mapper, CommonUtils commonUtils)
        {
            _response = new APIResponse();
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonUtils = commonUtils;
        }

        [HttpGet("GetDocuments", Name = "GetDocuments")]
        [Authorize(Roles = APIConstants.RecruiterRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetDocuments()
        {
            try
            {
                IEnumerable<CSVDocument> documents = await _unitOfWork.Documents.GetAll();
                if (documents == null)
                {
                    _response = APIResponse.NotFound(new List<string> { "No documents found" });
                }
                else
                {
                    IEnumerable<CSVDocumentDTO> documentDTO = _mapper.Map<IEnumerable<CSVDocumentDTO>>(documents);
                    _response = APIResponse.Ok(documentDTO);
                }
            }
            catch (Exception e)
            {
                _response = APIResponse.InternalServerError(new List<string>() { e.ToString() });
            }

            return _commonUtils.GetResult(this, _response);
        }

        [HttpPost("CreateDocument", Name = "CreateDocument")]
        [Authorize(Roles = APIConstants.RecruiterRole)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> CreateDocument([FromBody] CSVDocumentCreateDTO csvDocumentCreateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response = APIResponse.BadRequest(ModelState, new List<string> { $"Wrong input model {csvDocumentCreateDTO}" });
                }
                else
                {
                    string fileNameWithExtension = _commonUtils.AddFileExtension(csvDocumentCreateDTO.FileName, "csv");
                    var doesDocumentExist = await _unitOfWork.Documents.Get(f => f.FileName.ToLower() == fileNameWithExtension.ToLower());

                    List<List<string>> csvContent = new List<List<string>>(csvDocumentCreateDTO.CsvData);
                    csvContent.Insert(0, csvDocumentCreateDTO.CsvColumns);
                    if (doesDocumentExist != null)
                    {
                        _response = APIResponse.BadRequest(ModelState, new List<string> { $"The file with name {csvDocumentCreateDTO.FileName} already exists" });                    
                    }
                    else if (!_commonUtils.CheckCsvContentIsCorrect(csvContent))
                    {
                        _response = APIResponse.BadRequest(ModelState, new List<string> { $"All the lines need to have the same amount of fields than the number of columns. You defined the following columns {csvDocumentCreateDTO.CsvColumns}. " });
                    }
                    else 
                    {
                        bool isFileCreated = _commonUtils.CreateCSVFile(fileNameWithExtension, csvContent, out string filePath);
                        
                        if (isFileCreated)
                        {
                            CSVDocument csvDocument = new CSVDocument();
                            csvDocument.FileName = fileNameWithExtension;
                            csvDocument.FilePath = filePath;
                            csvDocument.UploadDate = DateTime.UtcNow;
                            csvDocument.LastModifiedDate = DateTime.UtcNow;
                            var creatorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                            csvDocument.RecruiterId = int.Parse(creatorId);

                            await _unitOfWork.Documents.Create(csvDocument);
                            _response = APIResponse.Created(_mapper.Map<CSVDocumentDTO>(csvDocument));
                        }
                        else
                        {
                            _response = APIResponse.BadRequest(ModelState, new List<string> { $"Error creating file {filePath}. " });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _response = APIResponse.InternalServerError(new List<string>() { e.ToString()});
            }

            return _commonUtils.GetResult(this, _response);
        }

        [HttpPost("UploadDocument", Name = "UploadDocument")]
        [Authorize(Roles = APIConstants.CandidateRole)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UploadDocument([FromBody] CSVDocumentUploadDTO csvDocumentUploadDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response = APIResponse.BadRequest(ModelState, new List<string> { $"Wrong input model {csvDocumentUploadDTO}" });
                }
                else
                {

                    var creatorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var previouslyUploadedDocument = await _unitOfWork.Documents.Get(f => f.CandidateId == int.Parse(creatorId));

                    if (previouslyUploadedDocument != null)
                    {
                        _commonUtils.DeleteCSVFile(previouslyUploadedDocument.FilePath);
                        await _unitOfWork.Documents.Delete(previouslyUploadedDocument);
                    }

                    bool isUploaded = _commonUtils.UploadCSVFile(csvDocumentUploadDTO.UploadedFilePath, out string tarjetPath);
                    if (isUploaded)
                    {
                        CSVDocument csvUploadDocument = new CSVDocument();
                        csvUploadDocument.FileName = Path.GetFileName(tarjetPath);
                        csvUploadDocument.FilePath = tarjetPath;
                        csvUploadDocument.UploadDate = DateTime.UtcNow;
                        csvUploadDocument.LastModifiedDate = DateTime.UtcNow;
                        csvUploadDocument.CandidateId = int.Parse(creatorId);

                        await _unitOfWork.Documents.Create(csvUploadDocument);
                        _response = APIResponse.Created(_mapper.Map<CSVDocumentDTO>(csvUploadDocument));
                    }
                    else
                    {
                        _response = APIResponse.InternalServerError(new List<string>() { $"Error uploading file with path {csvDocumentUploadDTO.UploadedFilePath}" });

                    }
                }
                
            }
            catch (Exception e)
            {
                _response = APIResponse.InternalServerError(new List<string>() { e.ToString() });
            }

            return _commonUtils.GetResult(this, _response);
        }

        [HttpDelete("DeleteDocument", Name = "DeleteDocument")]
        [Authorize(Roles = APIConstants.RecruiterRole)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteDocument(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response = APIResponse.BadRequest(null, new List<string> { $"Introduced id {id} is not valid. I has to be greater than 0." });
                }
                else
                {
                    var fileToDelete = await _unitOfWork.Documents.Get(f => f.Id == id);
                    
                    if (fileToDelete == null)
                    {
                        _response = APIResponse.NotFound(new List<string> { $"Document with id {id} not found" });
                    }
                    else
                    {

                        if (_commonUtils.DeleteCSVFile(fileToDelete.FilePath))
                        {
                            await _unitOfWork.Documents.Delete(fileToDelete);
                            _response = APIResponse.NoContent();
                        }
                        else
                        {
                            _response = APIResponse.InternalServerError(new List<string>() { $"Error deleting file {fileToDelete.FilePath}" });
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

        [HttpPut("UpdateDocument", Name = "UpdateDocument")]
        [Authorize(Roles = APIConstants.RecruiterRole)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDocument(int id, [FromBody] CSVDocumentEditDTO csvDocumentEditDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response = APIResponse.BadRequest(ModelState, new List<string> { $"Wrong input model {csvDocumentEditDTO}" });
                }
                else if (id != csvDocumentEditDTO.Id || id <= 0)
                {
                    _response = APIResponse.BadRequest(ModelState, new List<string> { $"Introduced id {id} is not valid. I has to be greater than 0 and equal to the model id." });
                }
                else
                {
                    var documentToUpdate = await _unitOfWork.Documents.Get(v => v.Id == csvDocumentEditDTO.Id, false);
                    if (documentToUpdate == null)
                    {
                        _response = APIResponse.BadRequest(ModelState, new List<string> { $"The document with id {csvDocumentEditDTO.Id} does not exists" });
                    }
                    else
                    {
                        List<List<string>> csvContent = new List<List<string>>(csvDocumentEditDTO.CsvData);
                        csvContent.Insert(0, csvDocumentEditDTO.CsvColumns);
                        if (!_commonUtils.CheckCsvContentIsCorrect(csvContent))
                        {
                            _response = APIResponse.BadRequest(ModelState, new List<string> { $"All the lines need to have the same amount of fields than the number of columns. You defined the following columns {csvDocumentEditDTO.CsvColumns}. " });
                        }
                        else
                        {
                            string fileNameWithExtension = _commonUtils.AddFileExtension(csvDocumentEditDTO.FileName, "csv");
                            string newPath = _commonUtils.RenameCSVFile(documentToUpdate.FilePath, fileNameWithExtension);
                            _commonUtils.WriteCSVFileContent(csvContent, newPath, false);

                            CSVDocument csvUpdateDocument = new CSVDocument();
                            csvUpdateDocument.FileName = Path.GetFileName(newPath);
                            csvUpdateDocument.FilePath = newPath;
                            csvUpdateDocument.LastModifiedDate = DateTime.UtcNow;
                            csvUpdateDocument.RecruiterId = documentToUpdate.RecruiterId;

                            await _unitOfWork.Documents.Update(csvUpdateDocument);
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
