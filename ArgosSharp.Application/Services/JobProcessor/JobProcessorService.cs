using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Application.Services.ArtifactsService;
using ArgosSharp.Application.Services.ExporterService;
using ArgosSharp.Application.UseCase.Scraper;
using ArgosSharp.Domain.Entity;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;
using System;

namespace ArgosSharp.Application.Services.JobProcessor
{
    internal class JobProcessorService(
        IJobExecutionRepository jobExecutionRepository,
        IScraperContext scraperStrategyContext,
        IJobRepository jobRepository,
        IExportService resultExportService,
        IArtifactsService artifactsService
    ) : IJobProcessorService
    {
        /// <inheritdoc cref="IJobProcessorService"/>
        public async Task ProcessJobAsync(JobExecution jobExecution)
        {
            await jobExecutionRepository.ProcessingAsync(jobExecution);
            // update start datetime on jobexecution object
            jobExecution.StartedAt = DateTime.UtcNow; // Temporary implementation
            await jobExecutionRepository.UpdateAsync(jobExecution);

            var job = await jobRepository.GetAsync(jobExecution.JobId) ?? throw new InvalidOperationException("Job is null");
            var result = await GetJobExecutionResult(job.JobType, jobExecution.Parameters) ?? throw new InvalidOperationException("Job execution result is null");
            var export = await resultExportService.ExportAsync(result, ExportFormat.CSV);

            await artifactsService.CreateAsync(jobExecution.Id, export.Content, export.FileName, export.ContentType, CancellationToken.None);
            await jobExecutionRepository.CompleteAsync(jobExecution);
        }

        private async Task<JobExecutionResult?> GetJobExecutionResult(JobType jobType, JobParameters jobParameters)
        {
            return jobType switch
            {
                JobType.NewsArticles => new JobExecutionResult
                {
                    JobType = typeof(NewsArticle),
                    Data = await scraperStrategyContext.GetNewsBySourceAsync(jobParameters)
                },
                JobType.JobPosting => new JobExecutionResult
                {
                    JobType = typeof(JobPosting),
                    Data = await scraperStrategyContext.GetJobsBySourceAsync(jobParameters)
                },
                _ => null,
            };
        }
    }
}
