using ArgosSharp.Domain.ValueObjects;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.Exceptions;
using ArgosSharp.Domain.Entity;

namespace ArgosSharp.Domain.Factories.JobFactory
{
    public class JobFactory : IJobFactory
    {
        public Job Create(string name, string term, IReadOnlyCollection<string> sources, JobParameters parameters, JobPriority priority, bool enabled)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(null, nameof(name));

            if (string.IsNullOrEmpty(term))
                throw new ArgumentException(null, nameof(term));

            return new Job(name, term, sources, parameters, priority, enabled);
        }
    }
}
