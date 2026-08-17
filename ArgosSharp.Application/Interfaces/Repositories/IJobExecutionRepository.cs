using ArgosSharp.Domain.Entity;

namespace ArgosSharp.Application.Interfaces.Repositories
{
    public interface IJobExecutionRepository
    {
        /// <summary>
        /// Adds a new job to the repository with an auto-incremented job ID.
        /// </summary>
        /// <param name="job">The job to add.</param>
        /// <returns>A completed task.</returns>
        Task AddAsync(JobExecution job);

        /// <summary>
        /// Retrieves a job from the repository by its job hash.
        /// </summary>
        /// <param name="jobHash">The unique hash identifier of the job to retrieve.</param>
        /// <returns>A task that returns the job if found, or null if not found.</returns>
        Task<JobExecution?> GetAsync(int id);

        /// <summary>
        /// Retrieves all jobs currently stored in the repository.
        /// </summary>
        /// <returns>A task that returns a list of all jobs.</returns>
        Task<List<JobExecution>> GetAllAsync();

        /// <summary>
        /// Updates an existing job in the repository.
        /// </summary>
        /// <param name="job">The job to update.</param>
        /// <returns>A completed task.</returns>
        Task UpdateAsync(JobExecution job);
    }
}
