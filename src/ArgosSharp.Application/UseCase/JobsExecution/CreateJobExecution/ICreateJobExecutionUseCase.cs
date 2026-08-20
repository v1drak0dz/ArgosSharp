using ArgosSharp.Application.Contracts.JobExecutions;
using ArgosSharp.Domain.Entity;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.UseCase.JobsExecution.CreateJobExecution
{
    public interface ICreateJobExecutionUseCase
    {
        Task<JobExecution> CreateJobExecution(CreateJobExecutionRequest createJobExecutionRequest);
    }
}
