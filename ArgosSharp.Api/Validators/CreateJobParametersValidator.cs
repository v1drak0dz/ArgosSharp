using ArgosSharp.Api.DTOs.Job.CreateJob;
using FluentValidation;

namespace ArgosSharp.Api.Validators
{
    internal class CreateJobParametersValidator : AbstractValidator<CreateJobParametersRequest>
    {
        internal CreateJobParametersValidator()
        {
            RuleFor(x => x.Depth)
                .GreaterThan(0).WithMessage("Depth must be greater than 0.");

            RuleFor(x => x.Sites)
                .NotNull().WithMessage("Sites list cannot be null.")
                .NotEmpty().WithMessage("Sites list cannot be empty.")
                .Must(sites => sites != null && sites.All(site => !string.IsNullOrWhiteSpace(site)))
                .WithMessage("Sites list cannot contain empty or whitespace strings.");
        }
    }
}
