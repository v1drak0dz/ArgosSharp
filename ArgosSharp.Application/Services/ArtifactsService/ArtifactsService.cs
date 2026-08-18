using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Application.Interfaces.Storage;
using ArgosSharp.Domain.Entity;

namespace ArgosSharp.Application.Services.ArtifactsService
{
    public class ArtifactsService(IArtifactStorage artifactStorage, IArtifactRepository artifactRepository) : IArtifactsService
    {
        public async Task<Artifact> CreateAsync(int jobExecutionId, Stream content, string fileName, string contentType, CancellationToken cancellation)
        {
            var storageKey = $"executions/{jobExecutionId}/{fileName}";

            var result = await artifactStorage.UploadAsync(content, storageKey, contentType, cancellation);

            var artifact = new Artifact(jobExecutionId, fileName, result.ContentType, result.Size, result.StorageKey);

            await artifactRepository.AddAsync(artifact);

            return artifact;
        }
    }
}
