using ArgosSharp.Domain.Enums;

namespace ArgosSharp.Domain.ValueObjects
{
    public class Job(string searchTerm, Dictionary<string, object> parameters, int depth, JobStatusEnum status)
    {
        public JobStatusEnum Status { get; set; } = status;
        public string SearchTerm { get; set; } = searchTerm;
        public Dictionary<string, object> Parameters { get; set; } = parameters;
        public int Depth { get; set; } = depth;
        public IEnumerable<object> Data { get; set; }
        public string Error { get; set; }
    }
}
