using ArgosSharp.Domain.Enums;

namespace ArgosSharp.Api.DTOs.Job
{
    public class JobResponseDTO(JobStatusEnum status, string hash, int job_id, IEnumerable<object> data)
    {
        public JobStatusEnum Status = status;
        public string JobHash = hash;
        public int JobId = job_id;
        public IEnumerable<object> Data = data;
    }
}
