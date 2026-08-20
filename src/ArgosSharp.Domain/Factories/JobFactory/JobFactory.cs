using ArgosSharp.Domain.ValueObjects;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.Entity;

namespace ArgosSharp.Domain.Factories.JobFactory
{
    public class JobFactory : IJobFactory
    {
        public Job Create(string name, JobType jobType, JobParameters parameters, JobPriority priority, bool enabled)
        {
            return string.IsNullOrEmpty(name) 
                ? throw new ArgumentException(null, nameof(name))
                : new Job(name, jobType, parameters, priority, enabled);
        }
    }
}
