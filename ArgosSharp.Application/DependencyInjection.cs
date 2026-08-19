using ArgosSharp.Application.Services.ArtifactsService;
using ArgosSharp.Application.Services.ExporterService;
using ArgosSharp.Application.Services.JobProcessor;
using ArgosSharp.Application.Services.JobQueue;
using ArgosSharp.Application.Services.JobWorker;
using ArgosSharp.Application.UseCase.Jobs.CreateJob;
using ArgosSharp.Application.UseCase.Jobs.GetJob;
using ArgosSharp.Application.UseCase.JobsExecution.CreateJobExecution;
using ArgosSharp.Application.UseCase.Scraper;
using Microsoft.Extensions.DependencyInjection;

namespace ArgosSharp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection service)
        {
            service.AddSingleton<IJobExecutionQueue, JobExecutionQueue>();

            service.AddScoped<IScraperContext, ScraperContext>();
            service.AddScoped<IJobProcessorService, JobProcessorService>();
            service.AddScoped<IArtifactsService, ArtifactsService>();
            service.AddScoped<IExportService, ExportService>();

            service.AddScoped<ICreateJobUseCase, CreateJobUseCase>();
            service.AddScoped<IGetJobUseCase, GetJobUseCase>();

            service.AddScoped<ICreateJobExecutionUseCase, CreateJobExecutionUseCase>();

            service.AddHostedService<JobWorker>();
            service.AddHostedService<JobWorker>();

            return service;
        }
    }
}

