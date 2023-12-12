using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentManagementAPI.Models.Entities
{
    public class Recruiter : User
    {
        public ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
        public ICollection<CSVDocument> Documents { get; set; } = new List<CSVDocument>();
    }
}
