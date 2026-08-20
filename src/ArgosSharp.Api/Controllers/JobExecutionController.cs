using ArgosSharp.Application.Contracts.JobExecutions;
using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Application.UseCase.JobsExecution.CreateJobExecution;
using ArgosSharp.Domain.Entity;
using Microsoft.AspNetCore.Mvc;

namespace ArgosSharp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobExecutionController(
        ICreateJobExecutionUseCase createJobExecutionUseCase,
        IJobExecutionRepository jobExecutionRepository
    ) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<JobExecution>> GetJobExecutionsAsync()
        {
            return await jobExecutionRepository.GetAllAsync();
        }

        [HttpPost]
        public async Task<ActionResult<JobExecution>> CreateJobExecution([FromBody] CreateJobExecutionRequest createJobExecutionRequest)
        {
            var execution = await createJobExecutionUseCase.CreateJobExecution(createJobExecutionRequest);
            return Ok(execution);
        }
    }
}
