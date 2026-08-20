namespace ArgosSharp.Domain.ValueObjects
{
    public sealed class JobParameters
    {
        public string Query { get; set; }
        public IReadOnlyCollection<string> Sources { get; set; }
        public int Depth { get; set; }

        private JobParameters() { }

        public JobParameters(string query, IReadOnlyCollection<string> sources, int depth)
        {
            Query = query;
            Sources = sources;
            Depth = depth;
        }
    }
}
