using ArgosSharp.Domain.Enums;

namespace ArgosSharp.Domain.Entity
{
    public sealed class JobExecution
    {
        public Guid Id { get; set; }
        public int JobId { get; set; }
        public JobStatusEnum JobStatus { get; set; }
        public int Attempt { get; set; }
        public string? WorkerId { get; set; }
        public DateTime QueuedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string? Error { get; set; }
    }
}
