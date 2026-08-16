using ArgosSharp.Application.Interfaces.Persistence;
using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Application.Interfaces.UnitOfWork;
using ArgosSharp.Application.Services.JobQueue;
using ArgosSharp.Application.Services.JobWorker;
using ArgosSharp.Domain.Factories.JobFactory;
using ArgosSharp.Infrastructure.Persistence;
using ArgosSharp.Infrastructure.Repositories;
using ArgosSharp.Infrastructure.UnitOfWork;

namespace ArgosSharp.Api.DependeciesInjection
{
    internal static class JobDependecies
    {
        internal static IServiceCollection AddJobDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IJobQueue, JobQueue>();
            
            services.AddScoped<IJobPersistence, JobPersistence>();
            services.AddScoped<IJobFactory, JobFactory>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobUnitOfWork, JobUnitOfWork>();
            
            // Trying to understand that this following 2 lines starts 2 workers
            services.AddHostedService<JobWorker>();
            services.AddHostedService<JobWorker>();

            return services;
        }
    }
}
