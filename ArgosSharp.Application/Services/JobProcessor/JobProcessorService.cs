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

                var sources = job.Parameters.Sites.AsEnumerable();
                var depth = job.Parameters.Depth;

                var data = await scraperProcessor.GetNoticias(job.SearchTerm, depth, sources);

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
