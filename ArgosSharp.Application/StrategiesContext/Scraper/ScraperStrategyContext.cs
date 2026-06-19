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

        public async Task<List<Noticia>> GetNoticiasBySourceAsync(ScraperSourceEnum scraperSource, string searchTerm, int depth) =>
            await _strategies[scraperSource.ToString()].ProcessScraperAsync(searchTerm, depth);
    }
}
