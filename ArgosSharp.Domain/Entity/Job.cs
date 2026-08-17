using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Domain.Entity
{
    public sealed class Job
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public JobParameters Parameters { get; set; }
        public JobPriority Priority { get; set; }
        public bool Enabled { get; set; }

        private Job() { }
        
        public Job(string name, JobParameters parameters, JobPriority priority, bool enabled)
        {
            Name = name;
            Parameters = parameters;
            Priority = priority;
            Enabled = enabled;
        }
    }
}
