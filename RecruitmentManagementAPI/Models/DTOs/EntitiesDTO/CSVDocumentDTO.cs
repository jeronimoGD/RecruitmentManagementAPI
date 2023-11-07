using RecruitmentManagementAPI.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentManagementAPI.Models.DTOs.EntitiesDTO
{
    public class CSVDocumentDTO
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public string FilePath { get; set; }
    }
}
