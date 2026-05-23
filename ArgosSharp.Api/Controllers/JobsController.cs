using ArgosSharp.Api.DTOs.Job;
using ArgosSharp.Application.Services.JobQueue;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.Factories.JobFactory;
using Microsoft.AspNetCore.Mvc;

namespace ArgosSharp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController(IJobQueue jobQueue, IJobFactory jobFactory) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<JobResponseDTO>> CreateJobAsync([FromBody] JobRequestDTO dto)
        {
            try
            {
                var job = jobFactory.Create(dto.searchTerm, dto.parameters);
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
