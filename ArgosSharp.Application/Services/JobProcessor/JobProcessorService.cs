using ArgosSharp.Application.Interfaces.UnitOfWork;
using ArgosSharp.Application.UseCase.Scraper;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.Model;

namespace ArgosSharp.Application.Services.JobProcessor
{
    public class JobProcessorService(IScraperProcessor scraperProcessor, IJobUnitOfWork jobUnitOfWork) : IJobProcessorService
    {

        /// <inheritdoc cref="IJobProcessorService"/>
        public async Task ProcessJobAsync(Job job)
        {
            try
            {
                await jobUnitOfWork.UpdateJobStatus(job, JobStatusEnum.Processing);

                var sources = job.Parameters.Sites.AsEnumerable();
                var depth = job.Parameters.Depth;

                var data = await scraperProcessor.GetNews(job.SearchTerm, depth, sources);

                job.Data = data;

                await jobUnitOfWork.UpdateJobStatus(job, JobStatusEnum.Completed);
            }
            catch (Exception ex)
            {
                job.Error = ex.Message;
                await jobUnitOfWork.UpdateJobStatus(job, JobStatusEnum.Failed);
            }
        }
    }
}
