using ArgosSharp.Application.Contracts.Jobs;
using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Application.Interfaces.UnitOfWork;
using ArgosSharp.Application.Services.JobQueue;
using ArgosSharp.Domain.Entity;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.Factories.JobFactory;

namespace ArgosSharp.Application.UseCase.Jobs.CreateJob
{
    public class CreateJobUseCase(IJobFactory jobFactory, IJobRepository jobRepository, IJobQueue jobQueue) : ICreateJobUseCase
    {
        /// <inheritdoc cref="ICreateJobUseCase"/>
        public async Task<Job> CreateJob(CreateJobRequest createJobRequest)
        {
            // Criar Job
            var job = jobFactory.Create(
                createJobRequest.Name,
                createJobRequest.Parameters,
                createJobRequest.Priority,
                createJobRequest.Enabled
            );
            // Persistir Job
            await jobRepository.AddAsync(job);
            // Enfileirar
            await jobQueue.EnqueueAsync(job);
            // Atualizar Status
            await jobRepository.UpdateAsync(job);
            // Retornar resposta
            return job;
        }
    }
}
