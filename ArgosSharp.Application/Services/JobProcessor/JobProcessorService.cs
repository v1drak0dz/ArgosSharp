using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Application.UseCase.Scraper;
using ArgosSharp.Domain.Entity;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Services.JobProcessor
{
    internal class JobProcessorService(
        IJobExecutionRepository jobExecutionRepository,
        IScraperContext scraperStrategyContext,
        IJobRepository jobRepository
    ) : IJobProcessorService
    {
        /// <inheritdoc cref="IJobProcessorService"/>
        public async Task ProcessJobAsync(JobExecution jobExecution)
        {
            await jobExecutionRepository.ProcessingAsync(jobExecution);
            // update start datetime on jobexecution object

            // Call Scraper Strategy based on source
            var job = await jobRepository.GetAsync(jobExecution.JobId) ?? throw new InvalidOperationException();
            switch (job.JobType)
            {
                case JobType.NewsArticles:
                    _ = new JobExecutionResult
                    {
                        JobType = JobType.NewsArticles,
                        Data = await scraperStrategyContext.GetNewsBySourceAsync(jobExecution.Parameters)
                    };
                    await jobExecutionRepository.CompleteAsync(jobExecution);
                    return;

                case JobType.JobPosting:
                    _ = new JobExecutionResult
                    {
                        JobType = JobType.JobPosting,
                        Data = await scraperStrategyContext.GetJobsBySourceAsync(jobExecution.Parameters)
                    };
                    await jobExecutionRepository.CompleteAsync(jobExecution);
                    return;
            }
        }
    }
}
