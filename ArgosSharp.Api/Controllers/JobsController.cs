using Microsoft.AspNetCore.Mvc;
using FluentValidation;

using ArgosSharp.Application.Contracts.Jobs;
using ArgosSharp.Application.UseCase.Jobs.CreateJob;
using ArgosSharp.Application.UseCase.Jobs.GetJob;
using ArgosSharp.Domain.Entity;

namespace ArgosSharp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController(
        ICreateJobUseCase createJobUseCase,
        IGetJobUseCase getJobUseCase
    ) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<Job>> GetJobsAsync()
        {
            return await getJobUseCase.GetJobsAsync();
        }

        [HttpPost]
        public async Task<ActionResult<CreateJobResponse>> CreateJobAsync([FromBody] CreateJobRequest createJobRequest)
        {
            //var validationResult = await createJobValidator.ValidateAsync(createJobRequest);
            //if (!validationResult.IsValid)
            //{
            //    return BadRequest(validationResult.Errors);
            //}

            var job = await createJobUseCase.CreateJob(createJobRequest);

            return Ok(job);
        }
    }
}
