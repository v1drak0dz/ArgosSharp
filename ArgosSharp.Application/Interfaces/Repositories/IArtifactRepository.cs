using ArgosSharp.Domain.Entity;

namespace ArgosSharp.Application.Interfaces.Repositories
{
    public interface IArtifactRepository
    {
        Task<IReadOnlyCollection<Artifact>> GetByJobExecutionIdAsync(int jobExecId);
        Task<Artifact?> GetByIdAsync(int id);
        Task AddAsync(Artifact artifact);
        Task DeleteAsync(Artifact artifact);
    }
}
