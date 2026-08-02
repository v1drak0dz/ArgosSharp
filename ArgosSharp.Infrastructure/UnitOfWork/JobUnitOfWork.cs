using ArgosSharp.Application.Interfaces.Persistence;
using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Application.Interfaces.UnitOfWork;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.Model;

namespace ArgosSharp.Infrastructure.UnitOfWork
{
    public class JobUnitOfWork(IJobRepository jobRepository, IJobPersistence jobPersistence) : IJobUnitOfWork
    {
        /// <inheritdoc cref="IJobUnitOfWork" />
        public async Task AddJobAsync(Job job)
        {
            await jobRepository.AddAsync(job);
            await jobPersistence.SaveAsync(jobRepository.GetSnapshot());
        }

        /// <inheritdoc cref="IJobUnitOfWork" />
        public async Task UpdateJobStatus(Job job, JobStatusEnum jobStatus)
        {
            job.Status = jobStatus;
            await jobRepository.UpdateAsync(job);
            await jobPersistence.SaveAsync(jobRepository.GetSnapshot());
        }

        /// <inheritdoc cref="IJobUnitOfWork" />
        public async Task UpdateJobAsync(Job job)
        {
            await jobRepository.UpdateAsync(job);
            await jobPersistence.SaveAsync(jobRepository.GetSnapshot());
        }

        /// <inheritdoc cref="IJobUnitOfWork" />
        public async Task InitializeAsync()
        {
            var jobs = await jobPersistence.LoadAsync();
            jobRepository.Load(jobs);
        }
    }
}
