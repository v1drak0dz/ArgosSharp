using ArgosSharp.Domain.ValueObjects;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.Model;
using ArgosSharp.Domain.Exceptions;

namespace ArgosSharp.Domain.Factories.JobFactory
{
    public class JobFactory : IJobFactory
    {
        public Job Create(string searchTerm, List<string> sites, int depth)
        {
            if (string.IsNullOrEmpty(searchTerm))
                throw new InvalidSearchTermException();

            if (depth <= 0)
                throw new InvalidDepthException();

            if (sites.Count == 0)
                throw new InvalidSitesException();

            return new Job(searchTerm, new JobParameters(sites: sites, depth: depth), JobStatusEnum.Created);
        }
    }
}
