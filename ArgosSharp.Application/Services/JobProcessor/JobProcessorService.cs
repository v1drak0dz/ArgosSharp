using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Application.UseCase.Scraper;
using ArgosSharp.Domain.Entity;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Services.JobProcessor
{
    internal class JobProcessorService(
        IJobExecutionRepository jobExecutionRepository,
        IScraperContext scraperStrategyContext
    ) : IJobProcessorService
    {
        /// <inheritdoc cref="IJobProcessorService"/>
        public async Task ProcessJobAsync(JobExecution jobExecution)
        {
            await jobExecutionRepository.ProcessingAsync(jobExecution);
            // update start datetime on jobexecution object

            // Call Scraper Strategy based on source
            var news = new List<NewsArticle>();
            var parameters = jobExecution.Parameters;
            foreach (var source in parameters.Sources)
                news.AddRange(await scraperStrategyContext.GetNewsBySourceAsync(source, parameters.Query, parameters.Depth));

            await jobExecutionRepository.CompleteAsync(jobExecution);
        }
    }
}
