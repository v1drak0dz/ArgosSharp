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
                // Problematic, JobId is being incremented after being dequeued
                // since we are responding before it being dequeued, how can I
                // treat this case of needing a JobId.

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
