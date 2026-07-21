using ArgosSharp.Domain.Enums;

namespace ArgosSharp.Domain.ValueObjects
{
    public class JobParameters(List<string> sites, int depth)
    {
        public List<string> Sites { get; set; } = sites;
        public int Depth { get; set; } = depth;
    }
}
