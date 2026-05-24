using ArgosSharp.Api.DTOs.Job;
using ArgosSharp.Application.Services.JobQueue;
using ArgosSharp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using ArgosSharp.Api.DTOs.Job.CreateJob;
using ArgosSharp.Api.Mappers.JobMapper;

namespace ArgosSharp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController(IJobQueue jobQueue, IJobMapper jobMapper) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<JobResponseDTO>> CreateJobAsync([FromBody] CreateJobRequest createJobDto)
        {
            try
            {
                var job = jobMapper.JobFromRequest(createJobDto);
                await jobQueue.EnqueueAsync(job);
                return Ok(new JobResponseDTO(JobStatusEnum.Created, Guid.NewGuid().ToString(), 1, []));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return NotFound();
            }
        }
    }
}
