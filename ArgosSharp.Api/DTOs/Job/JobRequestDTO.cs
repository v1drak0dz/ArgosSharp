using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Api.DTOs.Job
{
    public class JobRequestDTO
    {
        public string searchTerm { get; set; }
        public JobParameters parameters { get; set; }
    }
}
