using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Contracts.Jobs
{
    public sealed record CreateJobResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SearchTerm { get; set; }
        public IReadOnlyCollection<string> Sources { get; set; }
        public JobParameters JobParameters { get; set; }
        public JobPriority Priority { get; set; }
        public bool Enabled { get; set; }
    }
}
