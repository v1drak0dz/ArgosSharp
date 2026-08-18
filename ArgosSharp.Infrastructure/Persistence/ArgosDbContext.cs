using ArgosSharp.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace ArgosSharp.Infrastructure.Persistence
{
    public class ArgosDbContext(DbContextOptions<ArgosDbContext> options) : DbContext(options)
    {
        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<JobExecution> JobExecutions => Set<JobExecution>();
        public DbSet<Artifact> Artifacts => Set<Artifact>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ArgosDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
