using Microsoft.EntityFrameworkCore;
using RecruitmentManagementAPI.Models.Entities;

namespace RecruitmentManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Recruiter> Recruiters { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<CSVDocument> CsvDocuments{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recruiter>().HasData(
                new Recruiter()
                {
                    Id = 1,
                    Name = "DefaultRecruiter",
                    Email = "defaultRecruiter@gmail.com",
                    Password = "password",
                    Rol = "recruiter",
                    CreationTime = DateTime.UtcNow,
                    UpdateTime = DateTime.UtcNow }
                );
        }
    }
}
