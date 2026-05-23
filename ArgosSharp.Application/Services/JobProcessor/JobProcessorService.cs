using ArgosSharp.Application.UseCase.Scraper;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Services.JobProcessor
{
    public class JobProcessorService(IScraperProcessor scraperProcessor) : IJobProcessorService
    {

        public async Task ProcessJobAsync(Job job)
        {

            try
            {
                job.Status = JobStatusEnum.Processing;

                IEnumerable<ScraperSourceEnum> sources = [
                    ScraperSourceEnum.Caraguatatuba,
                ];

                var data = await scraperProcessor.GetNoticiasNoticias(job.SearchTerm, job.Depth, sources);

                job.Data = data;

                job.Status = JobStatusEnum.Completed;
            }
            catch (Exception ex)
            {
                job.Status = JobStatusEnum.Failed;
                job.Error = ex.Message;
            }
        }
    }
}
