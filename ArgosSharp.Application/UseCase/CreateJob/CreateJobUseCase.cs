using ArgosSharp.Application.Interfaces.UnitOfWork;
using ArgosSharp.Application.Services.JobQueue;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.Factories.JobFactory;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.UseCase.CreateJob
{
    public class CreateJobUseCase(IJobFactory jobFactory, IJobUnitOfWork jobUnitOfWork, IJobQueue jobQueue) : ICreateJobUseCase
    {
        /// <inheritdoc cref="ICreateJobUseCase"/>
        public async Task<Job> CreateJob(string term, List<string> sites, int depth)
        {
            // Criar Job
            var job = jobFactory.Create(term, sites, depth);
            // Persistir Job
            await jobUnitOfWork.AddJobAsync(job);
            // Enfileirar
            await jobQueue.EnqueueAsync(job);
            // Atualizar Status
            await jobUnitOfWork.UpdateJobStatus(job, JobStatusEnum.Enqueued);
            // Retornar resposta
            return job;
        }
    }
}
