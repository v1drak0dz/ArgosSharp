using ArgosSharp.Domain.Factories.JobExecutionFactory.cs;
using ArgosSharp.Domain.Factories.JobFactory;
using Microsoft.Extensions.DependencyInjection;

namespace ArgosSharp.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            services.AddScoped<IJobFactory, JobFactory>();
            services.AddScoped<IJobExecutionFactory, JobExecutionFactory>();

            return services;
        }
    }
}
