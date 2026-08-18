using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Domain.Entity;
using ArgosSharp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArgosSharp.Infrastructure.Repositories
{
    internal class ArtifactRepository(ArgosDbContext argosDbContext) : IArtifactRepository
    {
        public async Task<Artifact?> GetByIdAsync(int id) =>
            await argosDbContext.Artifacts.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IReadOnlyCollection<Artifact>> GetByJobExecutionIdAsync(int jobExecId) =>
            await argosDbContext.Artifacts.Where(x => x.JobExecutionId == jobExecId).ToListAsync();

        public async Task AddAsync(Artifact artifact)
        {
            await argosDbContext.Artifacts.AddAsync(artifact);
            await argosDbContext.SaveChangesAsync();
        }

        public Task DeleteAsync(Artifact artifact)
        {
            argosDbContext.Artifacts.Remove(artifact);
            return Task.CompletedTask;
        }
    }
}
