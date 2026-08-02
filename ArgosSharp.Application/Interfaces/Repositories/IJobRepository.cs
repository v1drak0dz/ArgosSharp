using ArgosSharp.Domain.Model;

namespace ArgosSharp.Application.Interfaces.Repositories
{
    public interface IJobRepository
    {
        /// <summary>
        /// Adds a new job to the repository with an auto-incremented job ID.
        /// </summary>
        /// <param name="job">The job to add.</param>
        /// <returns>A completed task.</returns>
        Task AddAsync(Job job);

        /// <summary>
        /// Retrieves a job from the repository by its job hash.
        /// </summary>
        /// <param name="jobHash">The unique hash identifier of the job to retrieve.</param>
        /// <returns>A task that returns the job if found, or null if not found.</returns>
        Task<Job?> GetAsync(Guid hash);

        /// <summary>
        /// Retrieves all jobs currently stored in the repository.
        /// </summary>
        /// <returns>A task that returns a list of all jobs.</returns>
        Task<List<Job>> GetAllAsync();

        /// <summary>
        /// Updates an existing job in the repository.
        /// </summary>
        /// <param name="job">The job to update.</param>
        /// <returns>A completed task.</returns>
        Task UpdateAsync(Job job);

        /// <summary>
        /// Loads a collection of jobs into the repository and updates the current job ID counter
        /// to the maximum job ID found in the loaded collection.
        /// </summary>
        /// <param name="jobs">The list of jobs to load into the repository.</param>
        void Load(List<Job> jobs);

        /// <summary>
        /// Returns a snapshot of all current job values in the repository.
        /// </summary>
        /// <returns>An enumerable collection of all jobs currently stored.</returns>
        IEnumerable<Job> GetSnapshot();
    }
}
