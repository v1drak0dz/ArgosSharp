using ArgosSharp.Application.Interfaces.Strategies;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.StrategiesContext.Scraper
{
    public class ScraperStrategyContext : IScraperStrategyContext
    {
        private readonly Dictionary<string, IScraperStrategy> _strategies;

        public ScraperStrategyContext(IEnumerable<IScraperStrategy> strategies) =>
            _strategies = strategies.ToDictionary(s => s.Name);

        /// <inheritdoc cref="IScraperStrategyContext"/>
        public async Task<List<Noticia>> GetNoticiasBySourceAsync(string scraperSource, string searchTerm, int depth) =>
            await _strategies[scraperSource].ProcessScraperAsync(searchTerm, depth);
    }
}