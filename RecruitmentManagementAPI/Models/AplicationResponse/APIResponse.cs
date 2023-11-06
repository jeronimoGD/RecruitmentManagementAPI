using Microsoft.AspNetCore.Mvc;
using System.Net;

using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentManagementAPI.Models;
using RecruitmentManagementAPI.Models.AplicationResponse;
using RecruitmentManagementAPI.Models.DTOs.EntitiesDTO;
using RecruitmentManagementAPI.Models.Entities;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;

namespace RecruitmentManagementAPI.Models.AplicationResponse
{
    public class APIResponse
    {
        public APIResponse()
        {
            StatusCode = HttpStatusCode.OK;
            IsSuccesful = true;
            ErrorMessages = new List<string>();
        }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccesful { get; set; }
        public List<string> ErrorMessages { get; set; }
        public object Result { get; set; }


        // More common success responses
        public static APIResponse Ok(object result)
        {
            return new APIResponse{ StatusCode = HttpStatusCode.OK, IsSuccesful = true, Result = result };
        }

        public static APIResponse Created(object result)
        {
            return new APIResponse { StatusCode = HttpStatusCode.Created, IsSuccesful = true, Result = result };
        }
        public static APIResponse NoContent()
        {
            return new APIResponse { StatusCode = HttpStatusCode.NoContent, IsSuccesful = true};
        }


        // More common error responses
        public static APIResponse BadRequest(object result, List<string> errorMessages)
        {
            return new APIResponse { StatusCode = HttpStatusCode.BadRequest, IsSuccesful = true, Result = result, ErrorMessages = errorMessages };
        }

        public static APIResponse NotFound(List<string> errorMessages)
        {
            return new APIResponse { StatusCode = HttpStatusCode.NotFound, IsSuccesful = true, Result = null, ErrorMessages = errorMessages };
        }

        public static APIResponse InternalServerError(List<string> errorMessages)
        {
            return new APIResponse { StatusCode = HttpStatusCode.InternalServerError, IsSuccesful = true, Result = null, ErrorMessages = errorMessages };
        }
    }
}
