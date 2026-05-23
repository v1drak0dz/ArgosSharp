using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Services.PersistenceService
{
    public interface IPersistenceService
    {
        Task SaveAsync(IEnumerable<Job> jobs);

        Task<List<Job>> LoadAsync();
    }
}
