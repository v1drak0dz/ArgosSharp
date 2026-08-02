using ArgosSharp.Application.Interfaces.Persistence;
using ArgosSharp.Domain.Model;
using System.Text.Json;

namespace ArgosSharp.Infrastructure.Persistence
{
    public class JobPersistence(string testFilePath) : IJobPersistence
    {
        private readonly string FilePath = string.IsNullOrEmpty(testFilePath) ? "jobs_history.json" : testFilePath;
        private readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };
        private readonly SemaphoreSlim _lock = new(1, 1);

        /// <inheritdoc cref="IJobPersistence">
        public async Task SaveAsync(IEnumerable<Job> jobs)
        {
            await _lock.WaitAsync();
            try
            {
                var json = JsonSerializer.Serialize(jobs, SerializerOptions);
                var tempFile = $"{FilePath}.tmp";

                await File.WriteAllTextAsync(tempFile, json);

                File.Move(tempFile, FilePath, true);
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <inheritdoc cref="IJobPersistence">
        public async Task<List<Job>> LoadAsync()
        {
            if (!File.Exists(FilePath))
                return [];

            var json = await File.ReadAllTextAsync(FilePath);

            return JsonSerializer.Deserialize<List<Job>>(json) ?? [];
        }
    }
}
