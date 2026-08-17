using ArgosSharp.Domain.Entity;
using System.Threading.Channels;

namespace ArgosSharp.Application.Services.JobQueue
{
    public class JobExecutionQueue : IJobExecutionQueue
    {
        private readonly Channel<JobExecution> queue = Channel.CreateUnbounded<JobExecution>();

        /// <inheritdoc cref="IJobExecutionQueue"/>
        public async Task EnqueueAsync(JobExecution job)
        {
            await queue.Writer.WriteAsync(job);
        }

        /// <inheritdoc cref="IJobExecutionQueue"/>
        public async Task<JobExecution> DequeueAsync(CancellationToken cancellationToken)
        {
            return await queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
