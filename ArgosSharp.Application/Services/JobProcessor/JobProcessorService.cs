using ArgosSharp.Application.Services.JobStore;
using ArgosSharp.Application.UseCase.Scraper;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Services.JobProcessor
{
    public class JobProcessorService(IScraperProcessor scraperProcessor, IJobStore jobStore) : IJobProcessorService
    {

        public async Task ProcessJobAsync(Job job)
        {
            try
            {
                job.Status = JobStatusEnum.Processing;

                await jobStore.UpdateAsync(job);

                var sources = job.Parameters.Sites.AsEnumerable();
                var depth = job.Parameters.Depth;

                var data = await scraperProcessor.GetNoticias(job.SearchTerm, depth, sources);

                job.Data = data;

                job.Status = JobStatusEnum.Completed;

                await jobStore.UpdateAsync(job);
            }
            catch (Exception ex)
            {
                job.Status = JobStatusEnum.Failed;
                job.Error = ex.Message;
                await jobStore.UpdateAsync(job);
            }
        }
    }
}
