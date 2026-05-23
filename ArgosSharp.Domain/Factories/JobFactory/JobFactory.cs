using ArgosSharp.Domain.ValueObjects;
using ArgosSharp.Domain.Enums;

namespace ArgosSharp.Domain.Factories.JobFactory
{
    public class JobFactory : IJobFactory
    {
        public Job Create(string searchTerm, JobParameters parameters)
        {
            return new Job(searchTerm, parameters, JobStatusEnum.Created);
        }
    }
}
