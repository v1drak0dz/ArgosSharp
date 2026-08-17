using ArgosSharp.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArgosSharp.Infrastructure.Persistence.Configurations
{
    internal class JobExecutionConfiguration : IEntityTypeConfiguration<JobExecution>
    {
        public void Configure(EntityTypeBuilder<JobExecution> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.JobId).IsRequired();
            builder.Property(x => x.JobStatus).IsRequired();
            builder.OwnsOne(x => x.Parameters, parameters =>
            {
                parameters.Property(p => p.Query);
                parameters.Property(p => p.Depth);
                parameters.Property(p => p.Sources)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)!);
            });
            builder.Property(x => x.QueuedAt).IsRequired();
            builder.Property(x => x.StartedAt);
            builder.Property(x => x.FinishedAt);
            builder.Property(x => x.Error);
        }
    }
}
