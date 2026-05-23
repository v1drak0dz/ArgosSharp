using ArgosSharp.Domain.Enums;

namespace ArgosSharp.Domain.ValueObjects
{
    public class JobParameters
    {
        public List<ScraperSourceEnum> Sites { get; set; } = [];
        public int Depth { get; set; } = -1;
    }
}
