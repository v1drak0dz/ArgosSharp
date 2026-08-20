using ArgosSharp.Application.Interfaces.Fetcher;
using ArgosSharp.Application.Interfaces.Parser;
using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Application.Interfaces.ResultExporters;
using ArgosSharp.Application.Interfaces.Scrapers;
using ArgosSharp.Application.Interfaces.Storage;
using ArgosSharp.Infrastructure.Http.Fetcher;
using ArgosSharp.Infrastructure.Http.Parser;
using ArgosSharp.Infrastructure.Persistence;
using ArgosSharp.Infrastructure.Persistence.Configurations;
using ArgosSharp.Infrastructure.Repositories;
using ArgosSharp.Infrastructure.ResultExporters;
using ArgosSharp.Infrastructure.Scrapers.JobPostings;
using ArgosSharp.Infrastructure.Scrapers.NewsArticles;
using ArgosSharp.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace ArgosSharp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ArgosDbContext>(options => options.UseNpgsql(DatabaseConfiguration.BuildConnectionString(configuration)));
            services.AddSingleton<IMinioClient>(_ =>
            {
                return new MinioClient()
                    .WithEndpoint(configuration["S3_ENDPOINT"])
                    .WithCredentials(configuration["S3_ACCESS_KEY"]!, configuration["S3_SECRET_KEY"]!)
                    .WithSSL(bool.Parse(configuration["S3_USE_SSL"] ?? "false"))
                    .Build();
            });

            services.AddScoped<IHttpFetcher, HttpClientFetcher>();
            services.AddScoped<IHtmlParser, AngleSharpHtmlParser>();

            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobExecutionRepository, JobExecutionRepository>();
            services.AddScoped<IArtifactRepository, ArtifactRepository>();

            services.AddScoped<IArtifactStorage, ArtifactStorage>();

            services.AddScoped<INewsArticlesScrapers, CaraguatatubaScraper>();
            services.AddScoped<INewsArticlesScrapers, SaoSebastiaoScraper>();
            services.AddScoped<INewsArticlesScrapers, UbatubaScraper>();

            services.AddScoped<IJobPostingsScrapers, IndeedScraper>();

            services.AddScoped<IResultExporter, CSVResultExporter>();

            return services;
        }
    }
}
