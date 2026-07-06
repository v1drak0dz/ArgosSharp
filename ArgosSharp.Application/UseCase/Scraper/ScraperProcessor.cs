using ArgosSharp.Application.StrategiesContext.Scraper;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.UseCase.Scraper
{
    public class ScraperProcessor(IScraperStrategyContext scraperStrategyContext) : IScraperProcessor
    {
        /// <inheritdoc cref = "IScraperProcessor" />
        public async Task<List<Noticia>> GetNoticias(string searchTerm, int depth, IEnumerable<string> scraperSources)
        {
            if (string.IsNullOrEmpty(searchTerm))
                throw new ArgumentNullException();

            if (int.IsNegative(depth))
                throw new ArgumentNullException();

            var noticias = new List<Noticia>();

            foreach (var source in scraperSources)
            {
                noticias.AddRange(await scraperStrategyContext.GetNoticiasBySourceAsync(source, searchTerm, depth));
            }

            return noticias;
        }
    }
}
