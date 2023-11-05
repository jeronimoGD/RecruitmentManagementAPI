using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentManagementAPI.Models.Entities
{
    public class Candidate : User
    {
        // Relations with the recruiter creator
        [ForeignKey("RecruiterCreator")]
        public int RecruiterId { get; set; }
        public Recruiter RecruiterCreator { get; set; }

        //Relations with the owned Document
        public CSVDocument? Document { get; set; }
    }
}
