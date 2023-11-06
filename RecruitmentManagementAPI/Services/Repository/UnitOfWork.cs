﻿using AutoMapper;
using Microsoft.Extensions.Options;
using RecruitmentManagementAPI.Data;
using RecruitmentManagementAPI.Models.Constants;
using RecruitmentManagementAPI.Models.Entities;
using RecruitmentManagementAPI.Services.Repository.IRepository;

namespace RecruitmentManagementAPI.Services.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private IRecruiterRepository _recruiters { get; set; }
        private IRepository<Candidate> _candidates { get; set; }
        private IRepository<CSVDocument> _documents { get; set; }

        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IOptions<APISettings> _apiSettings;

        public UnitOfWork(ApplicationDbContext context, IConfiguration conf, IMapper mapper, IOptions<APISettings> apiSettings)
        {
            _dbContext = context;
            _apiSettings = apiSettings;
            _mapper = mapper;
        }

        public IRecruiterRepository Recruiters
        {
            get
            {
                return _recruiters == null ? _recruiters = new RecruiterRepository(_dbContext, _mapper, _apiSettings) : _recruiters;
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
