using ArgosSharp.Domain.ValueObjects;
using ArgosSharp.Domain.Enums;

namespace ArgosSharp.Domain.Factories.JobFactory
{
    public class JobFactory : IJobFactory
    {
        public Job Create(string searchTerm, Dictionary<string, object> parameters)
        {
            return new Job(searchTerm, parameters, JobStatusEnum.Created);
        }
    }
}
