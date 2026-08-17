using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.UseCase.Scraper
{
    public interface IScraperContext
    {
        /// <summary>
        /// Retrieves news items from the specified scraper source for the given search term and depth.
        /// Looks up the appropriate strategy by source name and delegates the scraping to it.
        /// </summary>
        /// <param name="scraperSource">The name of the scraper source to use.</param>
        /// <param name="searchTerm">The search term to query.</param>
        /// <param name="depth">The scraping depth to apply.</param>
        /// <returns>A list of news items retrieved from the source.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the specified scraper source is not registered.</exception>
        Task<List<NewsArticle>> GetNewsBySourceAsync(string scraperSource, string searchTerm, int depth);
    }
}
