namespace ArgosSharp.Domain.ValueObjects
{
    public sealed record ResultExport
    {
        public Stream Content { get; init; }
        public string FileName { get; init; }
        public string ContentType { get; init; }
        public string Extension { get; init; }
    };
}
