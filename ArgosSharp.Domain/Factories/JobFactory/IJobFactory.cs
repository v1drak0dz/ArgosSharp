using ArgosSharp.Domain.Entity;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Domain.Factories.JobFactory
{
    public interface IJobFactory
    {
        Job Create(string name, JobParameters parameters, JobPriority priority, bool enabled);
    }
}
