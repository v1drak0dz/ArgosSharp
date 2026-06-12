using ArgosSharp.Application.Interfaces.Fetcher;
using ArgosSharp.Application.Interfaces.Parser;
using ArgosSharp.Application.Interfaces.Strategies;
using ArgosSharp.Domain.Annotations;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net;

namespace ArgosSharp.Infrastructure.Strategies.Scrapers
{
    [ScraperSourceAnnotation(ScraperSourceEnum.Ubatuba)]
    public class UbatubaScraper(ILogger<UbatubaScraper> _logger, IHttpFetcher _fetcher, IHtmlParser _parser) : IScraperStrategy
    {
        private const string BaseUrl = "https://www.ubatuba.sp.gov.br/page/{0}/?s=";
        private const string PaginationQuery = "ul[class*='pagenavi'] > li";
        private const string NewsQuery = "ul[class*='blog-items'] div[class*='search-item-content']";
        private const string TitleQuery = "h3 > a::text()";
        private const string LinkQuery = "h3 > a::attr(href)";
        private const string SummaryQuery = "div[class*='excerpt']::text()";
        private const string DateQuery = "time::text()";

        public string Name => "ubatuba";

        public async Task<List<Noticia>> ProcessScraperAsync(string searchTerm, int depth)
        {
            var news = new List<string>();
            var termParsed = WebUtility.UrlEncode(searchTerm);

            _logger.LogInformation("Starting scraping for term {SearchTerm} in Ubatuba", searchTerm);

            var firstPageHtml = await _fetcher.GetStringAsync(string.Format(BaseUrl, 1) + termParsed);
            var maxPage = GetPaginationIfExists(firstPageHtml);
            var limit = Math.Min(maxPage, depth);

            news.AddRange(GetNewsIfExists(firstPageHtml));

            for (var i = 2; i <= limit; i++)
            {
                var html = await _fetcher.GetStringAsync(string.Format(BaseUrl, i) + termParsed);
                var pageNews = GetNewsIfExists(html);
                if (pageNews.Count > 0)
                {
                    news.AddRange(pageNews);
                    _logger.LogDebug("{NewsCount} news found on page {Page}", pageNews.Count, i);
                }
                else
                {
                    _logger.LogWarning("No news found on page {Page}", i);
                }
            }

            _logger.LogInformation("Total news collected: {Total}", news.Count);

            var results = BuildResponseData(news);

            _logger.LogInformation("Scraping finished. {ProcessedCount} news processed.", results.Count);

            return results;
        }

        private int GetPaginationIfExists(string html)
        {
            var pagination = _parser.QueryTexts(html, PaginationQuery).ToList();
            if (pagination.Count > 1 && int.TryParse(pagination[^2], out int maxPage))
            {
                _logger.LogInformation("Total number of pages found: {PageQuantity}", maxPage);
                return maxPage;
            }
            _logger.LogWarning("Pagination not found. Assuming only one page.");
            return 1;
        }

        private List<string> GetNewsIfExists(string document)
        {
            var noticias = _parser.QueryTexts(document, NewsQuery).ToList();
            _logger.LogInformation("News found on page: {NewsCount}", noticias.Count);
            return noticias;
        }

        private List<Noticia> BuildResponseData(List<string> news)
        {
            List<Noticia> formattedNews = [];
            foreach (var item in news)
            {
                var title = _parser.QueryText(item, TitleQuery) ?? "No title";
                var link = _parser.QueryText(item, LinkQuery) ?? "No link";
                var summary = _parser.QueryText(item, SummaryQuery) ?? "";

                var rawDate = _parser.QueryText(item, DateQuery);
                DateTime.TryParseExact(rawDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);

                formattedNews.Add(
                    new Noticia(
                        title: title,
                        dateTime: date,
                        year: date != default ? date.Year : 0,
                        link: link,
                        @abstract: summary,
                        source: Name
                    )
                );

                _logger.LogDebug("News '{Title}' added.", title);
            }
            return formattedNews;
        }
    }
}
