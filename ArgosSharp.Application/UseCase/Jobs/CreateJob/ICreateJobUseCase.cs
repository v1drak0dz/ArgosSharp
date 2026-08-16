using ArgosSharp.Domain.Entity;
using ArgosSharp.Application.Contracts.Jobs;

namespace ArgosSharp.Application.UseCase.Jobs.CreateJob
{
    public interface ICreateJobUseCase
    {
        /// <summary>
        /// Creates a new job for the given search term and sites with the specified depth,
        /// persists it using the job unit of work, enqueues it for processing, updates
        /// its status to Enqueued and returns the created job.
        /// </summary>
        /// <param name="term">Search term for the job.</param>
        /// <param name="sites">List of site identifiers to scrape.</param>
        /// <param name="depth">Scraping depth to use for the job.</param>
        /// <returns>The created and persisted <see cref="Job"/> instance.</returns>
        Task<Job> CreateJob(CreateJobRequest createJobRequest);
    }
}
