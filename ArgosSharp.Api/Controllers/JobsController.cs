using ArgosSharp.Api.DTOs.Job;
using ArgosSharp.Application.UseCase.Scraper;
using ArgosSharp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ArgosSharp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController(IScraperProcessor _scraperProcessor) : ControllerBase
    {
        [HttpPost]
        public async void CreateJobAsync([FromBody] JobRequestDTO dto)
        {
            try
            {
                // CREATE JOB
                // _ = Task.Run(async () => { await _jobProcessorService.ProcessJobAsync(job);
                // Estudar implementacao de BackgroundService + Queue
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
