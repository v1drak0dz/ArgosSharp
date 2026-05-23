using ArgosSharp.Domain.ValueObjects;
using System.Threading.Channels;

namespace ArgosSharp.Application.Services.JobQueue
{
    public class JobQueue : IJobQueue
    {
        private readonly Channel<Job> queue = Channel.CreateUnbounded<Job>();

        public async Task EnqueueAsync(Job job)
        {
            await queue.Writer.WriteAsync(job);
        }

        public async Task<Job> DequeueAsync(CancellationToken cancellationToken)
        {
            return await queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
