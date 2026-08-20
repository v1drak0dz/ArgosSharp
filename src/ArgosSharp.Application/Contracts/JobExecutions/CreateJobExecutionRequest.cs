using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Contracts.JobExecutions
{
    public sealed record CreateJobExecutionRequest
    {
        public int JobId { get; set; }
        public JobParameters Parameters { get; set; }
    }
}
