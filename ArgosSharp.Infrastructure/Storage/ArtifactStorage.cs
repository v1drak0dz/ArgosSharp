using ArgosSharp.Application.Interfaces.Storage;
using ArgosSharp.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;

namespace ArgosSharp.Infrastructure.Storage
{
    internal class ArtifactStorage(IMinioClient minioClient, IConfiguration configuration) : IArtifactStorage
    {
        private readonly IMinioClient _minioClient = minioClient;
        private readonly string _bucket = configuration["S3_BUCKET"] ?? throw new InvalidOperationException("S3_BUCKET is not configured.");

        public async Task<ArtifactStorageResult> UploadAsync(Stream content, string storagePath, string contentType, CancellationToken cancellation = default)
        {
            var exists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket), cancellation);

            if (!exists)
                await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket), cancellation);

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucket)
                .WithObject(storagePath)
                .WithStreamData(content)
                .WithObjectSize(content.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs, cancellation);

            return new ArtifactStorageResult { StorageKey = storagePath, Size = content.Length, ContentType = contentType };
        }

        public async Task<Stream> DownloadAsync(
            string storageKey,
            CancellationToken cancellationToken = default)
        {
            var memoryStream = new MemoryStream();

            var args = new GetObjectArgs()
                .WithBucket(_bucket)
                .WithObject(storageKey)
                .WithCallbackStream(stream =>
                {
                    stream.CopyTo(memoryStream);
                });

            await _minioClient.GetObjectAsync(
                args,
                cancellationToken);

            memoryStream.Position = 0;

            return memoryStream;
        }

        public async Task DeleteAsync(
            string storageKey,
            CancellationToken cancellationToken = default)
        {
            var args = new RemoveObjectArgs()
                .WithBucket(_bucket)
                .WithObject(storageKey);

            await _minioClient.RemoveObjectAsync(
                args,
                cancellationToken);
        }
    }
}
