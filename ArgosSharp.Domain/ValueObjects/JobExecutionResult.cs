using ArgosSharp.Domain.Enums;

namespace ArgosSharp.Domain.ValueObjects
{
    public sealed record JobExecutionResult
    {
        public required Type JobType { get; init; }
        public required IEnumerable<object> Data { get; init; }
    }
}
