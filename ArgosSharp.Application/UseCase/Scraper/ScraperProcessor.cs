using ArgosSharp.Application.StrategiesContext.Scraper;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.UseCase.Scraper
{
    public class ScraperProcessor(IScraperStrategyContext scraperStrategyContext) : IScraperProcessor
    {
        /// <inheritdoc cref = "IScraperProcessor" />
        public async Task<List<News>> GetNews(string searchTerm, int depth, IEnumerable<string> scraperSources)
        {
            if (string.IsNullOrEmpty(searchTerm))
                throw new ArgumentNullException();

            if (int.IsNegative(depth))
                throw new ArgumentNullException();

            var news = new List<News>();

            foreach (var source in scraperSources)
            {
                news.AddRange(await scraperStrategyContext.GetNewsBySourceAsync(source, searchTerm, depth));
            }

            return news;
        }
    }
}
