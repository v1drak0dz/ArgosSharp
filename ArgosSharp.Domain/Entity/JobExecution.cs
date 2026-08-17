using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Domain.Entity
{
    public sealed class JobExecution
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public JobStatusEnum JobStatus { get; set; }
        public JobParameters Parameters { get; set; }
        public DateTime QueuedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string? Error { get; set; }

        private JobExecution() { }

        public JobExecution(int jobId, JobStatusEnum jobStatus, JobParameters parameters, DateTime queuedAt, DateTime? startedAt, DateTime? finishedAt, string? error)
        {
            JobId = jobId;
            JobStatus = jobStatus;
            Parameters = parameters;
            QueuedAt = queuedAt;
            StartedAt = startedAt;
            FinishedAt = finishedAt;
            Error = error;
        }
    }
}
