using ArgosSharp.Application.Interfaces.Fetcher;
using ArgosSharp.Application.Interfaces.Parser;
using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Application.Interfaces.Scrapers;
using ArgosSharp.Infrastructure.Http.Fetcher;
using ArgosSharp.Infrastructure.Http.Parser;
using ArgosSharp.Infrastructure.Persistence;
using ArgosSharp.Infrastructure.Persistence.Configurations;
using ArgosSharp.Infrastructure.Repositories;
using ArgosSharp.Infrastructure.Scrapers.JobPostings;
using ArgosSharp.Infrastructure.Scrapers.NewsArticles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArgosSharp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ArgosDbContext>(options => options.UseNpgsql(DatabaseConfiguration.BuildConnectionString(configuration)));

            services.AddScoped<IHttpFetcher, HttpClientFetcher>();
            services.AddScoped<IHtmlParser, AngleSharpHtmlParser>();

            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobExecutionRepository, JobExecutionRepository>();

            services.AddScoped<INewsArticlesScrapers, CaraguatatubaScraper>();
            services.AddScoped<INewsArticlesScrapers, SaoSebastiaoScraper>();
            services.AddScoped<INewsArticlesScrapers, UbatubaScraper>();

            services.AddScoped<IJobPostingsScrapers, IndeedScraper>();

            return services;
        }
    }
}
