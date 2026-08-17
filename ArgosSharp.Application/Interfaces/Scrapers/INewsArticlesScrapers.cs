using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Interfaces.Scrapers
{
    public interface INewsArticlesScrapers
    {
        string Name { get; set; }

        Task<List<NewsArticle>> ProcessScraperAsync(string searchTerm, int depth);
    }
}
