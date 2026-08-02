using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.UseCase.Scraper
{
    internal interface IScraperProcessor
    {
        /// <summary>
        /// Retrieves news items from the configured scraper sources for the
        /// provided search term and depth. The method queries each source via the
        /// scraper strategy context and aggregates the results into a single list.
        /// </summary>
        /// <param name="searchTerm">The search term to look for in each source.</param>
        /// <param name="depth">The scraping depth to apply when querying sources.</param>
        /// <param name="scraperSources">A collection of source identifiers to query.</param>
        /// <returns>A list of aggregated <see cref="News"/> instances from all sources.</returns>
        /// <exception cref="ArgumentNullException">Thrown when searchTerm is null or empty, or when depth is negative.</exception>
        Task<List<News>> GetNews(string searchTerm, int depth, IEnumerable<string> scraperSources);
    }
}
