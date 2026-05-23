using ArgosSharp.Application.Interfaces.Fetcher;
using ArgosSharp.Application.Interfaces.Parser;
using ArgosSharp.Application.Services.JobProcessor;
using ArgosSharp.Application.Services.JobQueue;
using ArgosSharp.Application.Services.JobWorker;
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
            services.AddScoped<CaraguatatubaScraper>();

            services.AddSingleton<JobQueue>();
            services.AddScoped<JobProcessorService>();

            // Trying to understand that this following 2 lines starts 2 workers
            services.AddHostedService<JobWorker>();
            services.AddHostedService<JobWorker>();

            return services;
        }
    }
}
