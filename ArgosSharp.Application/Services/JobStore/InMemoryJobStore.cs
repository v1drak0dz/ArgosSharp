using ArgosSharp.Application.Services.PersistenceService;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace ArgosSharp.Application.Services.JobStore
{
    public class InMemoryJobStore(IPersistenceService persistenceService) : IJobStore
    {
        private readonly ConcurrentDictionary<Guid, Job> _jobs = new();
        private readonly SemaphoreSlim saveLock = new(1, 1);

        public async Task AddJobAsync(Job job)
        {
            _jobs[job.JobHash] = job;

            await PersistAsync();
        }

        public Task<Job?> GetJobAsync(Guid jobHash)
        {
            _jobs.TryGetValue(jobHash, out var job);
            return Task.FromResult(job);
        }

        public Task<List<Job>> GetAllJobsAsync()
        {
            return Task.FromResult(_jobs.Values.ToList());
        }

        public async Task UpdateAsync(Job job)
        {
            _jobs[job.JobHash] = job;

            await PersistAsync();
        }

        public async Task InitializeAsync()
        {
            var jobs = await persistenceService.LoadAsync();

            foreach (var job in jobs)
            {
                if (job.Status == JobStatusEnum.Processing)
                {
                    job.Status = JobStatusEnum.Failed;
                    job.Error = "Application interrupted";
                }

                _jobs[job.JobHash] = job;
            }
        }

        private async Task PersistAsync()
        {
            await saveLock.WaitAsync();
            try
            {
                await persistenceService.SaveAsync(_jobs.Values);
            }
            finally
            {
                saveLock.Release();
            }
        }
    }
}
