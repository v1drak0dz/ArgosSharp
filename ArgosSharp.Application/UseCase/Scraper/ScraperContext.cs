using ArgosSharp.Application.Interfaces.Strategies;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.UseCase.Scraper
{
    public class ScraperContext : IScraperContext
    {
        private readonly Dictionary<string, IScraperStrategy> _strategies;

        public ScraperContext(IEnumerable<IScraperStrategy> strategies) =>
            _strategies = strategies.ToDictionary(s => s.Name);

        /// <inheritdoc cref="IScraperContext"/>
        public async Task<List<NewsArticles>> GetNewsBySourceAsync(string scraperSource, string searchTerm, int depth) =>
            await _strategies[scraperSource].ProcessScraperAsync(searchTerm, depth);
    }
}