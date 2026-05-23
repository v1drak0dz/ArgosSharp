using ArgosSharp.Application.UseCase.Scraper;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Services.JobProcessor
{
    public class JobProcessorService(IScraperProcessor scraperProcessor) : IJobProcessorService
    {
        private static readonly SemaphoreSlim semaphoreSlim = new(2);

        public async Task ProcessJobAsync(Job job)
        {
            await semaphoreSlim.WaitAsync();

            try
            {
                job.Status = JobStatusEnum.Created;

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
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
