using FluentValidation;

using ArgosSharp.Api.Validators;

namespace ArgosSharp.Api.DependeciesInjection
{
    internal static class ValidatorsDependencies
    {
        internal static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateJobValidator>();

            return services;
        }
    }
}
