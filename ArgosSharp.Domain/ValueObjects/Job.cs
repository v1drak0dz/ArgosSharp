using ArgosSharp.Domain.Enums;

namespace ArgosSharp.Domain.ValueObjects
{
    public class Job(string searchTerm, JobParameters parameters, JobStatusEnum status)
    {
        public JobStatusEnum Status { get; set; } = status;
        public string SearchTerm { get; set; } = searchTerm;
        public JobParameters Parameters { get; set; } = parameters;
        public IEnumerable<object> Data { get; set; } = [];
        public string Error { get; set; } = string.Empty;
        public Guid JobHash { get; set; } = Guid.NewGuid();
        public int JobId { get; set; } = 0;
    }
}
