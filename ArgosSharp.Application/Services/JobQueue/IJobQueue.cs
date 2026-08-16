using ArgosSharp.Domain.Entity;

namespace ArgosSharp.Application.Services.JobQueue
{
    public interface IJobQueue
    {
        /// <summary>
        /// Enqueues the specified job to be processed by consumers.
        /// </summary>
        /// <param name="job">The job to enqueue.</param>
        /// <returns>A task that represents the asynchronous enqueue operation.</returns>
        Task EnqueueAsync(Job job);

        /// <summary>
        /// Dequeues a job, waiting asynchronously until one is available or the provided
        /// cancellation token is triggered.
        /// </summary>
        /// <param name="cancellationToken">Token to observe while waiting for a job.</param>
        /// <returns>A task that represents the asynchronous dequeue operation returning the next <see cref="Job"/>.</returns>
        Task<Job> DequeueAsync(CancellationToken cancellationToken);
    }
}
