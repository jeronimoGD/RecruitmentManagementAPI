using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentManagementAPI.Models.Entities
{
    public class CSVDocument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        // Relacion con recruiter opcional
        public int? RecruiterId { get; set; }

        [ForeignKey("RecruiterId")]
        public Recruiter? RecruiterCreator { get; set; }

        // Relacion candidate opcional
        public int? CandidateId { get; set; }

        [ForeignKey("CandidateId")]
        public Candidate? CandidateCreator { get; set; }
    }
}
