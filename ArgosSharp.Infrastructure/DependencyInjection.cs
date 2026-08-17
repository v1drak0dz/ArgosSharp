using ArgosSharp.Application.Interfaces.Fetcher;
using ArgosSharp.Application.Interfaces.Parser;
using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Application.Interfaces.Strategies;
using ArgosSharp.Infrastructure.Http.Fetcher;
using ArgosSharp.Infrastructure.Http.Parser;
using ArgosSharp.Infrastructure.Persistence;
using ArgosSharp.Infrastructure.Persistence.Configurations;
using ArgosSharp.Infrastructure.Repositories;
using ArgosSharp.Infrastructure.Scrapers.NewsArticle;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArgosSharp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //var provider = configuration["DATABASE_PROVIDER"];
            //Console.WriteLine($"Vicente tentando conectar usando {provider}");
            //switch (configuration["DATABASE_PROVIDER"])
            //{
            //    case "postgres":
                    services.AddDbContext<ArgosDbContext>(options => options.UseNpgsql(DatabaseConfiguration.BuildConnectionString(configuration)));
            //        break;

            //    default:
            //        services.AddDbContext<ArgosDbContext>(options => options.UseSqlite("Data Source=./data.db"));
            //            break;
            //}
            
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IHttpFetcher, HttpClientFetcher>();
            services.AddScoped<IHtmlParser, AngleSharpHtmlParser>();
            services.AddScoped<IScraperStrategy, CaraguatatubaScraper>();
            
            return services;
        }
    }
}
