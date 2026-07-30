using ArgosSharp.Api.DTOs.Job;
using Microsoft.AspNetCore.Mvc;
using ArgosSharp.Api.DTOs.Job.CreateJob;
using ArgosSharp.Application.UseCase.CreateJob;
using FluentValidation;
using ArgosSharp.Api.Validators;

namespace ArgosSharp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController(ICreateJobUseCase createJobUseCase, IValidator<CreateJobRequest> createJobValidator, IValidator<CreateJobParametersRequest> createJobParamsValidator) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<JobResponseDTO>> CreateJobAsync([FromBody] CreateJobRequest createJobDto)
        {
            var validationResult = await createJobValidator.ValidateAsync(createJobDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            validationResult = await createJobParamsValidator.ValidateAsync(createJobDto.Parameters);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var job = await createJobUseCase.CreateJob(
                createJobDto.SearchTerm,
                createJobDto.Parameters.Sites,
                createJobDto.Parameters.Depth
            );

            return Ok(new JobResponseDTO(job.Status, job.JobHash.ToString(), job.JobId, job.Data));
        }
    }
}
