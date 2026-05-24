using ArgosSharp.Application.Interfaces.Fetcher;
using ArgosSharp.Application.Interfaces.Parser;
using ArgosSharp.Application.Interfaces.Strategies;
using ArgosSharp.Application.Services.JobQueue;
using ArgosSharp.Application.Services.JobStore;
using ArgosSharp.Application.Services.JobWorker;
using ArgosSharp.Application.Services.PersistenceService;
using ArgosSharp.Domain.Factories.JobFactory;
using ArgosSharp.Infrastructure.Http.Fetcher;
using ArgosSharp.Infrastructure.Http.Parser;
using ArgosSharp.Infrastructure.Strategies.Scrapers;

namespace ArgosSharp.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IHttpFetcher, HttpClientFetcher>();
            services.AddScoped<IHtmlParser, AngleSharpHtmlParser>();
            services.AddScoped<IScraperStrategy,CaraguatatubaScraper>();

            services.AddSingleton<IJobQueue, JobQueue>();

            services.AddScoped<IPersistenceService, PersistenceService>();
            services.AddScoped<IJobFactory, JobFactory>();
            services.AddScoped<IJobStore, InMemoryJobStore>();

            // Trying to understand that this following 2 lines starts 2 workers
            services.AddHostedService<JobWorker>();
            services.AddHostedService<JobWorker>();

            return services;
        }
    }
}
