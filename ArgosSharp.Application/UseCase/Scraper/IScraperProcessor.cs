using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.UseCase.Scraper
{
    public interface IScraperProcessor
    {
        Task<List<Noticia>> GetNoticiasNoticias(string searchTerm, int depth, IEnumerable<ScraperSourceEnum> scraperSources);
    }
}
