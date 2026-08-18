using ArgosSharp.Domain.Entity;

namespace ArgosSharp.Application.Services.ArtifactsService
{
    public interface IArtifactsService
    {
        Task<Artifact> CreateAsync(int jobExecutionId, Stream content, string fileName, string contentType, CancellationToken cancellation);
    }
}
