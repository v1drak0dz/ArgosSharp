using System.Collections.Concurrent;
using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Infrastructure.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly ConcurrentDictionary<Guid, Job> _jobs = new();
        private int currentJobId = 0;

        public Task AddAsync(Job job)
        {
            job.JobId = Interlocked.Increment(ref currentJobId);
            _jobs[job.JobHash] = job;
            return Task.CompletedTask;
        }

        public Task<Job?> GetAsync(Guid jobHash)
        {
            _jobs.TryGetValue(jobHash, out var job);
            return Task.FromResult(job);
        }

        public Task<List<Job>> GetAllAsync()
        {
            return Task.FromResult(_jobs.Values.ToList());
        }

        public Task UpdateAsync(Job job)
        {
            _jobs[job.JobHash] = job;
            return Task.CompletedTask;
        }

        public void Load(List<Job> jobs)
        {
            currentJobId = jobs.Count > 0 ? jobs.Max(x => x.JobId) : 0;

            foreach (var job in jobs)
                _jobs[job.JobHash] = job;
        }

        public IEnumerable<Job> GetSnapshot() =>
            _jobs.Values;
    }
}
