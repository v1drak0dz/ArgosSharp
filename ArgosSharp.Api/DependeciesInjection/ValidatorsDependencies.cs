using FluentValidation;

using ArgosSharp.Api.Validators;

namespace ArgosSharp.Api.DependeciesInjection
{
    public static class ValidatorsDependencies
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateJobValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateJobParametersValidator>();

            return services;
        }
    }
}
