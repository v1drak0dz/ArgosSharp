using ArgosSharp.Application.Services.JobProcessor;
using ArgosSharp.Application.Services.JobQueue;
using Microsoft.Extensions.Hosting;

namespace ArgosSharp.Application.Services.JobWorker
{
    public class JobWorker(IJobQueue jobQueue, IJobProcessorService jobProcessorService) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var job = await jobQueue.DequeueAsync(stoppingToken);

                await jobProcessorService.ProcessJobAsync(job);
            }
        }
    }
}
