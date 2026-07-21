using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Interfaces.UnitOfWork
{
    public  interface IJobUnitOfWork
    {
        /// <summary>
        /// Adds a new job to the repository and persists all jobs to storage.
        /// </summary>
        /// <param name="job">The job to add.</param>
        /// <returns>A task representing the asynchronous add and persist operation.</returns>
        Task AddJobAsync(Job job);

        /// <summary>
        /// Updates the status of the specified job and persists all jobs to storage.
        /// </summary>
        /// <param name="job">The job to update.</param>
        /// <param name="jobStatus">The new job status to set.</param>
        /// <returns>A task representing the asynchronous update and persist operation.</returns>
        Task UpdateJobStatus(Job job, JobStatusEnum jobStatus);

        /// <summary>
        /// Updates the specified job in the repository and persists all jobs to storage.
        /// </summary>
        /// <param name="job">The job to update.</param>
        /// <returns>A task representing the asynchronous update and persist operation.</returns>
        Task UpdateJobAsync(Job job);

        /// <summary>
        /// Initializes the unit of work by loading persisted jobs from storage into the repository.
        /// Call this once at application startup to restore previous job state.
        /// </summary>
        /// <returns>A task representing the asynchronous initialization operation.</returns>
        Task InitializeAsync();
    }
}
