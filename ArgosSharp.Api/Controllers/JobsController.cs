using ArgosSharp.Api.DTOs.Job;
using Microsoft.AspNetCore.Mvc;
using ArgosSharp.Api.DTOs.Job.CreateJob;
using ArgosSharp.Application.UseCase.CreateJob;

namespace ArgosSharp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController(ICreateJobUseCase createJobUseCase) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<JobResponseDTO>> CreateJobAsync([FromBody] CreateJobRequest createJobDto)
        {
            try
            {
                var job = await createJobUseCase.CreateJob(
                    createJobDto.SearchTerm,
                    createJobDto.Parameters.Sites,
                    createJobDto.Parameters.Depth
                );

                // Try/Catch and Logging via Middleware
                return Ok(new JobResponseDTO(job.Status, job.JobHash.ToString(), job.JobId, job.Data));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return NotFound();
            }
        }
    }
}
