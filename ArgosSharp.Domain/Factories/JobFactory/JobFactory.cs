using ArgosSharp.Domain.ValueObjects;
using ArgosSharp.Domain.Enums;

namespace ArgosSharp.Domain.Factories.JobFactory
{
    public class JobFactory : IJobFactory
    {
        public Job Create(string searchTerm, List<string> sites, int depth)
        {
            return new Job(searchTerm, new JobParameters(sites: sites, depth: depth), JobStatusEnum.Created);
        }
    }
}
