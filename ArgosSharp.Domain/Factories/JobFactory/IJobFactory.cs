using ArgosSharp.Domain.Entity;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Domain.Factories.JobFactory
{
    public interface IJobFactory
    {
        Job Create(string name, string term, IReadOnlyCollection<string> sources, JobParameters parameters, JobPriority priority, bool enabled);
    }
}
