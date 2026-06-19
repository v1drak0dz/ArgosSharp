using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Interfaces.Persistence
{
    public interface IJobPersistence
    {
        Task SaveAsync(IEnumerable<Job> jobs);
        Task<List<Job>> LoadAsync();
    }
}
