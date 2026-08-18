namespace ArgosSharp.Domain.Entity
{
    public sealed class Artifact
    {
        public int Id { get; set; }
        public int JobExecutionId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public string StoragePath { get; set; }

        private Artifact() { }

        public Artifact(int jobExecId, string fileName, string contentType, long size, string storagePath)
        {
            JobExecutionId = jobExecId;
            FileName = fileName;
            ContentType = contentType;
            Size = size;
            StoragePath = storagePath;
        }
    }
}
