using ArgosSharp.Domain.Enums;

namespace ArgosSharp.Domain.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScraperSourceAnnotation(ScraperSourceEnum scraperSourceEnum) : Attribute
    {
        public ScraperSourceEnum ScraperSource { get; } = scraperSourceEnum;
    }
}
