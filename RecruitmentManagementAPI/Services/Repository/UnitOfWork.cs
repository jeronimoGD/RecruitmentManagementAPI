using RecruitmentManagementAPI.Data;
using RecruitmentManagementAPI.Models.Entities;
using RecruitmentManagementAPI.Services.Repository.IRepository;

namespace RecruitmentManagementAPI.Services.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private IRepository<Recruiter> _recruiters { get; set; }
        private IRepository<Candidate> _candidates { get; set; }
        private IRepository<CSVDocument> _documents { get; set; }

        public ApplicationDbContext _dbContext;
        public UnitOfWork(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        public IRepository<Recruiter> Recruiters
        {
            get
            {
                return _recruiters == null ? _recruiters = new Repository<Recruiter>(_dbContext) : _recruiters;
            }
        }

        public IRepository<Candidate> Candidates
        {
            get
            {
                return _candidates == null ? _candidates = new Repository<Candidate>(_dbContext) : _candidates;
            }
        }

        public IRepository<CSVDocument> Documents
        {
            get
            {
                return _documents == null ? _documents = new Repository<CSVDocument>(_dbContext) : _documents;
            }
        }
        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
