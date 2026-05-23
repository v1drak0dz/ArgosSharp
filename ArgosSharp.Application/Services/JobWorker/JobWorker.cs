using ArgosSharp.Application.Services.JobProcessor;
using ArgosSharp.Application.Services.JobQueue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ArgosSharp.Application.Services.JobWorker
{
    public class JobWorker(IJobQueue jobQueue, IServiceScopeFactory serviceScopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var job = await jobQueue.DequeueAsync(stoppingToken);

                using var scope = serviceScopeFactory.CreateScope();

                var processor = scope.ServiceProvider.GetRequiredService<IJobProcessorService>();

                await processor.ProcessJobAsync(job);
            }
        }
    }
}
