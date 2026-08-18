namespace ArgosSharp.Domain.ValueObjects
{
    public sealed record ArtifactStorageResult
    {
        public required string StorageKey { get; set; }
        public required long Size { get; set; }
        public required string ContentType { get; set; }
    }
}
