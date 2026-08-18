using ArgosSharp.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace ArgosSharp.Infrastructure.Persistence.Configurations
{
    internal class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.JobType)
                .IsRequired();

            builder.OwnsOne(x => x.Parameters, parameters =>
            {
                parameters.Property(p => p.Query);
                parameters.Property(p => p.Depth);
                parameters.Property(p => p.Sources)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)!);
            });

            builder.Property(p => p.Priority)
                .IsRequired();

            builder.Property(x => x.Enabled)
                .IsRequired();
        }
    }
}
