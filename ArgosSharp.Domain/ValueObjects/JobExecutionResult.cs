using ArgosSharp.Domain.Enums;

namespace ArgosSharp.Domain.ValueObjects
{
    public sealed record JobExecutionResult
    {
        public required JobType JobType { get; init; }
        public required object Data { get; init; }
    }
}
