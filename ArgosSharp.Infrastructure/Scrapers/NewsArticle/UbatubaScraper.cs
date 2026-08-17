using ArgosSharp.Application.Interfaces.Fetcher;
using ArgosSharp.Application.Interfaces.Parser;
using ArgosSharp.Application.Interfaces.Strategies;
using ArgosSharp.Domain.ValueObjects;
using ArgosSharp.Infrastructure.Mapper;
using ArgosSharp.Infrastructure.Utils;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ArgosSharp.Infrastructure.Scrapers.NewsArticle
{
    public class UbatubaScraper(ILogger<UbatubaScraper> _logger, IHttpFetcher _fetcher, IHtmlParser _parser) : IScraperStrategy
    {
        public string Name { get; set; } = "ubatuba";

        private const string BaseUrl = "https://www.ubatuba.sp.gov.br/?s=";
        private const string PageBaseUrl = "https://www.ubatuba.sp.gov.br/page/{0}/?s=";

        private static readonly ScraperSelectors Selectors = new()
        {
            Pagination = "ul[class*='pagenavi'] > li",
            News = "ul[class*='blog-items'] div[class*='search-item-content']",
            Title = "h3 > a::text()",
            Link = "h3 > a::attr(href)",
            Abstract = "div[class*='excerpt']::text()",
            Date = "time::text()"
        };

        public async Task<List<NewsArticles>> ProcessScraperAsync(string searchTerm, int depth)
        {
            var news = new List<string>();
            var termParsed = WebUtility.UrlEncode(searchTerm);

            _logger.LogInformation("Starting scraping for term {SearchTerm} in Ubatuba", searchTerm);

            var firstPageHtml = await _fetcher.GetStringAsync(BaseUrl + termParsed);
            var maxPage = GetPaginationIfExists(firstPageHtml);
            var limit = Math.Min(maxPage, depth);

            news.AddRange(GetNewsIfExists(firstPageHtml));
            news.AddRange(await GetNewsFromPagination(termParsed, limit));

            _logger.LogInformation("Total news collected: {Total}", news.Count);

            return [..news.Select(x => NewsMapper.Map(x, _parser, Selectors, Name))];
        }

        private int GetPaginationIfExists(string html)
        {
            var pagination = _parser.QueryTexts(html, Selectors.Pagination).ToList();

            var maxPage = PaginationExtractor.GetMaxPage(pagination);

            _logger.LogInformation("Detected {PageQuantity} pages from pagination", maxPage);

            return maxPage;
        }

        private List<string> GetNewsIfExists(string document)
        {
            var news = _parser.QueryTexts(document, Selectors.News).ToList();
            _logger.LogInformation("News found on page: {NewsCount}", news.Count);
            return news;
        }

        private async Task<List<string>> GetNewsFromPagination(string termParsed, int limit)
        {
            List<string> newsRaw = [];
            for (var i = 2; i <= limit; i++)
            {
                var html = await _fetcher.GetStringAsync(string.Format(PageBaseUrl, i) + termParsed);
                var news = GetNewsIfExists(html);
                if (news.Count > 0)
                    newsRaw.AddRange(news);
            }
            return newsRaw;
        }
    }
}
