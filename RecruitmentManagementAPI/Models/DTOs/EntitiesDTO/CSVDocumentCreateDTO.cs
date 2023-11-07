﻿using System.ComponentModel.DataAnnotations;

namespace RecruitmentManagementAPI.Models.DTOs.EntitiesDTO
{
    public class CSVDocumentCreateDTO
    {
        [Required]
        public string FileName { get; set; }
        [Required]
        public List<string> CsvColumns { get; set; }
        [Required]
        public List<List<string>> CsvData { get; set; }
    }
}
