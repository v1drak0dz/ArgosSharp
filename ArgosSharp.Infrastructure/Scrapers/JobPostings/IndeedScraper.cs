using ArgosSharp.Application.Interfaces.Fetcher;
using ArgosSharp.Application.Interfaces.Scrapers;
using ArgosSharp.Domain.ValueObjects;
using ArgosSharp.Infrastructure.Mapper;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ArgosSharp.Infrastructure.Scrapers.JobPostings
{
    internal partial class IndeedScraper(
        IHttpFetcher fetcher,
        ILogger<IndeedScraper> logger)
    : IJobPostingsScrapers
    {
        public string Name { get; set; } = "indeed";

        private const string BaseUrl = "https://br.indeed.com";

        public async Task<List<JobPosting>> ProcessScraperAsync(
            string searchTerm,
            int depth)
        {
            var html = await fetcher.GetStringAsync(
                $"{BaseUrl}/jobs?q={Uri.EscapeDataString(searchTerm)}");

            var jobs = ExtractJobsFromResults(html);

            logger.LogInformation(
                "Found {Count} jobs in Indeed",
                jobs.Count);

            return jobs;
        }

        private List<JobPosting> ExtractJobsFromResults(string html)
        {
            var jobs = new List<JobPosting>();

            var match = JobsResults().Match(html);

            if (!match.Success)
                return jobs;

            var rawResults = match.Groups[1].Value;

            using var document = JsonDocument.Parse(rawResults);

            foreach (var item in document.RootElement.EnumerateArray())
            {
                var title = item.GetProperty("title").GetString();
                var link = item.GetProperty("link").GetString();
                var snippet = item.GetProperty("snippet").GetString();

                jobs.Add(
                    JobPostingMapper.Map(
                        title ?? string.Empty,
                        NormalizeLink(link),
                        CleanText(snippet),
                        Name));
            }

            return jobs;
        }

        private static string CleanText(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return WebUtility.HtmlDecode(text)
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Trim();
        }

        private static string NormalizeLink(string? link)
        {
            if (string.IsNullOrWhiteSpace(link))
                return string.Empty;

            if (link.StartsWith("http"))
                return link;

            return $"https://br.indeed.com{link}";
        }

        [GeneratedRegex(@"""results"":\s*(\[[\s\S]*?\])", RegexOptions.Singleline)]
        private static partial Regex JobsResults();
    }
}
