using ArgosSharp.Application.Interfaces.Scrapers;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.UseCase.Scraper
{
    public class ScraperContext(
        IEnumerable<INewsArticlesScrapers> newsArticlesScrapers,
        IEnumerable<IJobPostingsScrapers> jobPostingsScrapers
    ) : IScraperContext
    {
        private readonly Dictionary<string, INewsArticlesScrapers> NewsStrategies = newsArticlesScrapers.ToDictionary(s => s.Name);
        private readonly Dictionary<string, IJobPostingsScrapers> JobsStrategies = jobPostingsScrapers.ToDictionary(s => s.Name);

        /// <inheritdoc cref="IScraperContext"/>
        public async Task<List<NewsArticle>> GetNewsBySourceAsync(JobParameters jobParameters)
        {
            var newsArticles = new List<NewsArticle>();
            foreach (var source in jobParameters.Sources)
                newsArticles.AddRange(await NewsStrategies[source].ProcessScraperAsync(jobParameters.Query, jobParameters.Depth));
            return newsArticles;
        }

        public async Task<List<JobPosting>> GetJobsBySourceAsync(JobParameters jobParameters)
        {
            var jobsPostings = new List<JobPosting>();
            foreach (var source in jobParameters.Sources)
                jobsPostings.AddRange(await JobsStrategies[source].ProcessScraperAsync(jobParameters.Query, jobParameters.Depth));
            return jobsPostings;
        }
    }
}