using ArgosSharp.Api.DTOs.Job.CreateJob;
using FluentValidation;

namespace ArgosSharp.Api.Validators
{
    public class CreateJobValidator : AbstractValidator<CreateJobRequest>
    {
        public CreateJobValidator()
        {
            RuleFor(x => x.SearchTerm)
                .NotEmpty().WithMessage("SearchTerm is required.");
            RuleFor(x => x.Parameters)
                .NotNull().WithMessage("Parameters are required.");
        }
    }
}
