using ArgosSharp.Application.Interfaces.Scrapers;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.UseCase.Scraper
{
    public class ScraperContext : IScraperContext
    {
        private readonly Dictionary<string, INewsArticlesScrapers> _strategies;

        public ScraperContext(IEnumerable<INewsArticlesScrapers> strategies) =>
            _strategies = strategies.ToDictionary(s => s.Name);

        /// <inheritdoc cref="IScraperContext"/>
        public async Task<List<NewsArticle>> GetNewsBySourceAsync(string scraperSource, string searchTerm, int depth) =>
            await _strategies[scraperSource].ProcessScraperAsync(searchTerm, depth);
    }
}