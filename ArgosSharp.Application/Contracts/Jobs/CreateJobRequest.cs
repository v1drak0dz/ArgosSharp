using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Contracts.Jobs
{
    public sealed record CreateJobRequest
    {
        public string Name { get; set; }
        public JobParameters Parameters { get; set; }
        public JobPriority Priority { get; set; }
        public bool Enabled { get; set; } = true;
    }
}
