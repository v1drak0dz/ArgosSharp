using ArgosSharp.Api.Middlewares;

namespace ArgosSharp.Api.DependeciesInjection
{
    internal static class MiddlewareDependencies
    {
        internal static IServiceCollection AddMiddlewareDependencies(this IServiceCollection services)
        {
            services.AddScoped<MainMiddleware, MainMiddleware>();
            return services;
        }
    }
}
