using ArgosSharp.Domain.Model;
using System.Threading.Channels;

namespace ArgosSharp.Application.Services.JobQueue
{
    public class JobQueue : IJobQueue
    {
        private readonly Channel<Job> queue = Channel.CreateUnbounded<Job>();

        /// <inheritdoc cref="IJobQueue"/>
        public async Task EnqueueAsync(Job job)
        {
            await queue.Writer.WriteAsync(job);
        }

        /// <inheritdoc cref="IJobQueue"/>
        public async Task<Job> DequeueAsync(CancellationToken cancellationToken)
        {
            return await queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
