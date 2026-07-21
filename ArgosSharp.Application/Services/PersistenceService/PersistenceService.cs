using ArgosSharp.Domain.ValueObjects;
using System.Text.Json;

namespace ArgosSharp.Application.Services.PersistenceService
{
    public class PersistenceService(string testFilePath) : IPersistenceService
    {
        private readonly string FilePath = string.IsNullOrEmpty(testFilePath) ? "jobs_history.json" : testFilePath;
        private readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented= true};

        public async Task SaveAsync(IEnumerable<Job> jobs)
        {
            var json = JsonSerializer.Serialize(jobs, SerializerOptions);
            var tempFile = $"{FilePath}.tmp";

            await File.WriteAllTextAsync(tempFile, json);

            File.Move(tempFile, FilePath, true);
        }

        public async Task<List<Job>> LoadAsync()
        {
            if (!File.Exists(FilePath))
                return [];

            var json = await File.ReadAllTextAsync(FilePath);

            return JsonSerializer.Deserialize<List<Job>>(json) ?? [];
        }
    }
}
