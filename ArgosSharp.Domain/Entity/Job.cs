using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Domain.Entity
{
    public sealed class Job(string name, string searchTerm, IReadOnlyCollection<string> sources, JobParameters parameters, JobPriority priority, bool enabled)
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = name;
        public string SearchTerm { get; set; } = searchTerm;
        public IReadOnlyCollection<string> Sources { get; set; } = sources;
        public JobParameters Parameters { get; set; } = parameters;
        public JobPriority Priority { get; set; } = priority;
        public bool Enabled { get; set; } = enabled;
    }
}
