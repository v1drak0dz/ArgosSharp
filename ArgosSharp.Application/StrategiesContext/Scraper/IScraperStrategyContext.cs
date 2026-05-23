using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.StrategiesContext.Scraper
{
    public interface IScraperStrategyContext
    {
        Task<List<Noticia>> GetNoticiasBySourceAsync(ScraperSourceEnum scraperSource, string searchTerm, int depth);
    }
}
