using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Application.Services.ArtifactsService;
using ArgosSharp.Application.Services.ExporterService;
using ArgosSharp.Application.UseCase.Scraper;
using ArgosSharp.Domain.Entity;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

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

            // Call Scraper Strategy based on source
            var job = await jobRepository.GetAsync(jobExecution.JobId) ?? throw new InvalidOperationException();

            JobExecutionResult? result = null;
            switch (job.JobType)
            {
                case JobType.NewsArticles:
                    result = new JobExecutionResult
                    {
                        JobType = typeof(NewsArticle),
                        Data = await scraperStrategyContext.GetNewsBySourceAsync(jobExecution.Parameters)
                    };
                    break;

                case JobType.JobPosting:
                    result = new JobExecutionResult
                    {
                        JobType = typeof(JobPosting),
                        Data = await scraperStrategyContext.GetJobsBySourceAsync(jobExecution.Parameters)
                    };
                    break;
            }

            if (result == null)
                throw new InvalidOperationException("Job execution result is null.");

            var export = await resultExportService.ExportAsync(result, ExportFormat.JSON);

            await artifactsService.CreateAsync(jobExecution.Id, export.Content, export.FileName, export.ContentType, CancellationToken.None);

            await jobExecutionRepository.CompleteAsync(jobExecution);
        }
    }
}
