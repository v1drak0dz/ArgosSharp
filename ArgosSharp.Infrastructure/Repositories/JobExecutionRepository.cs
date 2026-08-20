using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Domain.Entity;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArgosSharp.Infrastructure.Repositories
{
    public class JobExecutionRepository(ArgosDbContext argosDbContext) : IJobExecutionRepository
    {
        public async Task AddAsync(JobExecution job)
        {
            argosDbContext.JobExecutions.Add(job);
            await argosDbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(JobExecution job)
        {
            argosDbContext.Update(job);
            await argosDbContext.SaveChangesAsync();
        }

        public async Task EnqueuedAsync(JobExecution job)
        {
            job.JobStatus = JobStatusEnum.Enqueued;
            await UpdateAsync(job);
        }

        public async Task CompleteAsync(JobExecution job)
        {
            job.FinishedAt = DateTime.Now;
            job.JobStatus = JobStatusEnum.Completed;
            await UpdateAsync(job);
        }

        public async Task FailAsync(JobExecution job)
        {
            job.FinishedAt = DateTime.Now;
            job.JobStatus = JobStatusEnum.Failed;
            await UpdateAsync(job);
        }

        public async Task ProcessingAsync(JobExecution job)
        {
            job.StartedAt = DateTime.Now;
            job.JobStatus = JobStatusEnum.Processing;
            await UpdateAsync(job);
        }

        public async Task<JobExecution?> GetAsync(int Id) =>
            await argosDbContext.JobExecutions.FirstOrDefaultAsync(x => x.Id == Id);

        public async Task<List<JobExecution>> GetAllAsync() =>
            await argosDbContext.JobExecutions.ToListAsync();
    }
}
