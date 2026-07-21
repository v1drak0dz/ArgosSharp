using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Application.UseCase.Scraper;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Services.JobProcessor
{
    public class JobProcessorService(IScraperProcessor scraperProcessor, IJobRepository jobStore) : IJobProcessorService
    {

        public async Task ProcessJobAsync(Job job)
        {
            try
            {
                await UpdateJobStatus(job, JobStatusEnum.Processing);

                var sources = job.Parameters.Sites.AsEnumerable();
                var depth = job.Parameters.Depth;

                var data = await scraperProcessor.GetNoticias(job.SearchTerm, depth, sources);

                job.Data = data;

                await UpdateJobStatus(job, JobStatusEnum.Completed);
            }
            catch (Exception ex)
            {
                job.Error = ex.Message;
                await UpdateJobStatus(job, JobStatusEnum.Failed);
            }
        }

        private async Task UpdateJobStatus(Job job, JobStatusEnum status)
        {
            job.Status = status;
            await jobStore.UpdateAsync(job);
        }
    }
}
