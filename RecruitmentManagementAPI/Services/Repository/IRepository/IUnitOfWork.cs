using RecruitmentManagementAPI.Models.Entities;

namespace RecruitmentManagementAPI.Services.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public IRecruiterRepository Recruiters { get; }
        public ICandidateRepository Candidates { get; }
        public IRepository<CSVDocument> Documents { get; }
        public void Save();
    }
}
