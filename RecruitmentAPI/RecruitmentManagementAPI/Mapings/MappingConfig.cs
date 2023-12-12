using AutoMapper;
using RecruitmentManagementAPI.Models.DTOs.EntitiesDTO;
using RecruitmentManagementAPI.Models.Entities;

namespace MagicVilla_API.Mapings
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Recruiter, UserDTO>().ReverseMap();
            CreateMap<Recruiter, UserCreateDTO>().ReverseMap();
            CreateMap<Recruiter, UserUpdateDTO>().ReverseMap();

            CreateMap<Candidate, UserDTO>().ReverseMap();
            CreateMap<Candidate, CandidateDTO>().ReverseMap();
            CreateMap<Candidate, UserUpdateDTO>().ReverseMap();
            CreateMap<Candidate, CandidateCreateDTO>().ReverseMap();

            CreateMap<CSVDocument, CSVDocumentDTO>().ReverseMap();
        }
    }
}
