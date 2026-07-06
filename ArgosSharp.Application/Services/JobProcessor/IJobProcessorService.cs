using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Services.JobProcessor
{
    public interface IJobProcessorService
    {
        /// <summary>
        /// Processes the provided job by performing the following steps:
        /// 1. Updates the job status to Processing via the job unit of work.
        /// 2. Runs the configured scraper to collect data based on the job's SearchTerm, Depth, and Sites.
        /// 3. Stores the retrieved data in <see cref="Job.Data"/> and updates the job status again.
        /// If an exception occurs during processing, the exception message is recorded in <see cref="Job.Error"/>
        /// and the job status is updated via the job unit of work.
        /// </summary>
        /// <param name="job">The job to process.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task ProcessJobAsync(Job job);
    }
}
