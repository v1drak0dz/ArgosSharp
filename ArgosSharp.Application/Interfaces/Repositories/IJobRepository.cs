using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Interfaces.Repositories
{
    public interface IJobRepository
    {
        Task AddAsync(Job job);
        Task<Job?> GetAsync(Guid hash);
        Task<List<Job>> GetAllAsync();
        Task UpdateAsync(Job job);

        IEnumerable<Job> GetSnapshot();
        void Load(List<Job> jobs);
    }
}
