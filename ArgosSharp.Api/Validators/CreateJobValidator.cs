using ArgosSharp.Api.DTOs.Job.CreateJob;
using FluentValidation;

namespace ArgosSharp.Api.Validators
{
    internal class CreateJobValidator : AbstractValidator<CreateJobRequest>
    {
        internal CreateJobValidator()
        {
            RuleFor(x => x.SearchTerm)
                .NotEmpty().WithMessage("SearchTerm is required.");
            RuleFor(x => x.Parameters)
                .NotNull().WithMessage("Parameters are required.");
        }
    }
}
