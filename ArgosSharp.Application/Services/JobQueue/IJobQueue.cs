using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Services.JobQueue
{
    public interface IJobQueue
    {
        Task EnqueueAsync(Job job);
        Task<Job> DequeueAsync(CancellationToken cancellationToken);
    }
}
