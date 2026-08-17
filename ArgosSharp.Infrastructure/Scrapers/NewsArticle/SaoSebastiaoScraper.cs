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
    public class SaoSebastiaoScraper(ILogger<SaoSebastiaoScraper> _logger, IHttpFetcher _fetcher, IHtmlParser _parser) : IScraperStrategy
    {
        public string Name { get; set; } = "sao_sebastiao";

        private const string BaseUrl = "https://www.saosebastiao.sp.gov.br/";
        private const string BaseSearch = BaseUrl + "noticia-lista.asp?idTitulo=";

        private static readonly ScraperSelectors Selectors = new()
        {
            Pagination = "div[id*='news_paging'] > li",
            News = "div[id*='page-content'] > article",
            Date = "div[class*='notice-date']::text()",
            Title = "h2 > a::text()",
            Abstract = "",
            Link = "h2 > a::attr(href)"
        };

        public async Task<List<NewsArticles>> ProcessScraperAsync(string searchTerm, int depth)
        {
            var news = new List<string>();
            var termParsed = WebUtility.UrlEncode(searchTerm);

            _logger.LogInformation("Initianting data gettering using term {SearchTerm} in São Sebastião", searchTerm);

            var html = await _fetcher.GetStringAsync($"{BaseSearch}{termParsed}");
            var maxPage = GetPaginationIfExists(html);
            var limit = Math.Min(maxPage, depth);

            news.AddRange(GetNewsIfExists(html));
            news.AddRange(await GetNewsFromPagination(termParsed, limit));

            return [.. news.Select(x => NewsMapper.Map(x, _parser, Selectors, Name))];
        }

        private int GetPaginationIfExists(string html)
        {
            var pagination = _parser.QueryTexts(html, Selectors.Pagination).ToList();

            var maxPage = PaginationExtractor.GetMaxPage(pagination);
            
            _logger.LogInformation("Found {PageQuantity} pages to scrape", maxPage);
            
            return maxPage;
        }

        private List<string> GetNewsIfExists(string document)
        {
            var news = _parser.QueryTexts(document, Selectors.News).ToList();

            _logger.LogInformation("Found {NewsCount} news", news.Count);
            
            return news;
        }

        private async Task<List<string>> GetNewsFromPagination(string term, int maxPage)
        {
            List<string> newsRaw = [];
            for (var i = 2; i <= maxPage; i++)
            {
                var html = await _fetcher.GetStringAsync($"{BaseSearch}{term}&pg={i}");
                var news = GetNewsIfExists(html);
                if (news.Count > 0)
                    newsRaw.AddRange(news);
            }
            return newsRaw;
        }
    }
}
