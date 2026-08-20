using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Interfaces.Storage
{
    public interface IArtifactStorage
    {
        Task<ArtifactStorageResult> UploadAsync(
            Stream content,
            string storagePath,
            string contentType,
            CancellationToken cancellationToken
        );

        Task<Stream> DownloadAsync(
            string storageKey,
            CancellationToken cancellationToken
        );

        Task DeleteAsync(
            string storageKey,
            CancellationToken cancellationToken
        );
    }
}
