using ArgosSharp.Domain.Model;

namespace ArgosSharp.Application.Interfaces.Persistence
{
    public interface IJobPersistence
    {
        /// <summary>
        /// Saves the provided collection of jobs to a JSON file with thread-safe locking.
        /// Uses atomic file operations (write to temporary file, then move) to prevent corruption.
        /// </summary>
        /// <param name="jobs">The collection of jobs to persist to disk.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        Task SaveAsync(IEnumerable<Job> jobs);

        /// <summary>
        /// Asynchronously loads all jobs from the persisted JSON file.
        /// Returns an empty list if the file does not exist.
        /// </summary>
        /// <returns>A list of deserialized Job instances from the file, or an empty list if the file does not exist.</returns>
        Task<List<Job>> LoadAsync();
    }
}
