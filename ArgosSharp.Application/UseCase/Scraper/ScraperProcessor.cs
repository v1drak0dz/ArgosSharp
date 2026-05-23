using ArgosSharp.Application.StrategiesContext.Scraper;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.UseCase.Scraper
{
    public class ScraperProcessor(IScraperStrategyContext scraperStrategyContext) : IScraperProcessor
    {
        public async Task<List<Noticia>> GetNoticiasNoticias(string searchTerm, int depth, IEnumerable<ScraperSourceEnum> scraperSources)
        {
            if (string.IsNullOrEmpty(searchTerm))
                throw new ArgumentNullException();

            if (int.IsNegative(depth))
                throw new ArgumentNullException();

            var noticias = new List<Noticia>();

            // source, string to enum convertion

            foreach (var source in scraperSources)
            {
                noticias.AddRange(await scraperStrategyContext.GetNoticiasBySourceAsync(source, searchTerm, depth));
            }

            return noticias;
        }
    }
}
