using ArgosSharp.Domain.Enums;

namespace ArgosSharp.Domain.ValueObjects
{
    public class JobParameters(List<ScraperSourceEnum> sites, int depth)
    {
        public List<ScraperSourceEnum> Sites { get; set; } = sites;
        public int Depth { get; set; } = depth;
    }
}
