namespace ArgosSharp.Domain.ValueObjects
{
    public sealed record JobPosting
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
    }
}
