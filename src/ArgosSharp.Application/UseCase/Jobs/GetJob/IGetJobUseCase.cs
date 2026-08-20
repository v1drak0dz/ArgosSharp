using ArgosSharp.Application.Contracts.Jobs;
using ArgosSharp.Domain.Entity;

namespace ArgosSharp.Application.UseCase.Jobs.GetJob
{
    public interface IGetJobUseCase
    {
        Task<IEnumerable<Job>> GetJobsAsync();
    }
}
