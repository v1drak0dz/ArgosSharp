using ArgosSharp.Application.Interfaces.Fetcher;
using ArgosSharp.Application.Interfaces.Parser;
using ArgosSharp.Application.Interfaces.Strategies;
using ArgosSharp.Domain.ValueObjects;
using System.Net;
using Microsoft.Extensions.Logging;
using ArgosSharp.Infrastructure.Utils;
using ArgosSharp.Infrastructure.Mapper;

namespace ArgosSharp.Infrastructure.Strategies.Scrapers
{
    public class CaraguatatubaScraper(IHttpFetcher _fetcher, IHtmlParser _parser, ILogger<CaraguatatubaScraper> _logger) : IScraperStrategy
    {
        public string Name { get; set; } = "caraguatatuba";

        private const string BaseUrl = "https://www.caraguatatuba.sp.gov.br/pmc";

        private static readonly ScraperSelectors Selectors = new()
        {
            Date = "span[class*='created-at']::text()",
            Title = "h5 > a::text()",
            Link = "h5 > a::attr(href)",
            Abstract = "div[class*='news-text'] > p::text()",
            Pagination = "ul[class*='pagination'] > li",
            News = "div[id*='latestNews'] > div[class*='row']"
        };

        public async Task<List<Noticia>> ProcessScraperAsync(string searchTerm, int depth)
        {
            var news = new List<string>();

            var termParsed = WebUtility.UrlEncode(searchTerm);

            _logger.LogInformation("Initiating data gathering using term {SearchTerm} in Caraguatatuba", searchTerm);

            var html = await _fetcher.GetStringAsync($"{BaseUrl}/?s={termParsed}");

            var maxPage = GetPaginationIfExists(html);
            var limit = Math.Min(maxPage, depth);

            news.AddRange(GetNewsIfExists(html));
            news.AddRange(await GetNewsFromPagination(termParsed, limit));

            return [.. news.Select(x => NoticiaMapper.Map(x, _parser, Selectors, Name))];
        }

        private int GetPaginationIfExists(string document)
        {
            var pagination = _parser.QueryTexts(document, Selectors.Pagination)?.ToList() ?? [];

            var maxPage = PaginationExtractor.GetMaxPage(pagination);

            _logger.LogInformation("Detected {PageQuantity} pages from pagination", maxPage);

            return maxPage;
        }

        private List<string> GetNewsIfExists(string document)
        {
            var noticias = _parser.QueryTexts(document, Selectors.News).ToList();

            _logger.LogInformation("Found {NewsCount} news", noticias.Count);

            return noticias;
        }

        private async Task<List<string>> GetNewsFromPagination(string term, int maxPage)
        {
            List<string> newsRaw = [];
            for (var i = 2; i <= maxPage; i++)
            {
                var html = await _fetcher.GetStringAsync($"{BaseUrl}/page/{i}/?s={term}");
                var news = GetNewsIfExists(html);
                if (news.Count > 0)
                    newsRaw.AddRange(news);
            }
            return newsRaw;
        }
    }
}
