using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Interfaces.Scrapers
{
    public interface IJobPostingsScrapers
    {
        string Name { get; set; }
        Task<List<JobPosting>> ProcessScraperAsync(string searchTerm, int depth);
    }
}
