using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Services.JobStore
{
    public interface IJobStore
    {
        Task AddJobAsync(Job job);
        Task<Job?> GetJobAsync(Guid jobhash);
        Task<List<Job>> GetAllJobsAsync();
        Task UpdateAsync(Job job);
        Task InitializeAsync();
    }
}
