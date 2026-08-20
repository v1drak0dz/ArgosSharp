using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Domain.Entity;

namespace ArgosSharp.Application.UseCase.Jobs.GetJob
{
    public class GetJobUseCase(IJobRepository jobRepository) : IGetJobUseCase
    {
        public async Task<IEnumerable<Job>> GetJobsAsync()
        {
            return await jobRepository.GetAllAsync();
        }
    }
}
