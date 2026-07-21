using ArgosSharp.Application.Interfaces.Persistence;
using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Application.Interfaces.UnitOfWork;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Infrastructure.UnitOfWork
{
    public class JobUnitOfWork(IJobRepository jobRepository, IJobPersistence jobPersistence) : IJobUnitOfWork
    {
        public async Task AddJobAsync(Job job)
        {
            await jobRepository.AddAsync(job);
            await jobPersistence.SaveAsync(jobRepository.GetSnapshot());
        }

        public async Task InitializeAsync()
        {
            var jobs = await jobPersistence.LoadAsync();
            jobRepository.Load(jobs);
        }
    }
}
