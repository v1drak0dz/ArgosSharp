using ArgosSharp.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArgosSharp.Infrastructure.Persistence.Configurations
{
    internal class ArtifactsConfiguration : IEntityTypeConfiguration<Artifact>
    {
        public void Configure(EntityTypeBuilder<Artifact> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.JobExecutionId).IsRequired();
            builder.Property(x => x.ContentType).IsRequired();
            builder.Property(x => x.StoragePath).IsRequired();
            builder.Property(x => x.FileName).IsRequired();
            builder.Property(x => x.Size).IsRequired();
        }
    }
}
