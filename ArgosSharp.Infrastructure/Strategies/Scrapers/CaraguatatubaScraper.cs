using ArgosSharp.Application.Interfaces.Fetcher;
using ArgosSharp.Application.Interfaces.Parser;
using ArgosSharp.Application.Interfaces.Strategies;
using ArgosSharp.Domain.ValueObjects;
using ArgosSharp.Domain.Annotations;
using ArgosSharp.Domain.Enums;
using System.Net;
using System.Globalization;
using Microsoft.Extensions.Logging;


namespace ArgosSharp.Infrastructure.Strategies.Scrapers
{
    [ScraperSourceAnnotation(ScraperSourceEnum.Caraguatatuba)]
    public class CaraguatatubaScraper(IHttpFetcher _fetcher, IHtmlParser _parser, ILogger<CaraguatatubaScraper> _logger) : IScraperStrategy
    {
        private const string BaseUrl = "https://www.caraguatatuba.sp.gov.br/pmc";
        public string Name => "caraguatatuba";

        public async Task<List<Noticia>> ProcessScraperAsync(string searchTerm, int depth)
        {
            var news = new List<string>();

            var termParsed = WebUtility.UrlEncode(searchTerm);

            _logger.LogInformation("Initianting data gettering using term {SearchTerm} in Caraguatatuba", searchTerm);

            var html = await _fetcher.GetStringAsync($"{BaseUrl}/?s={termParsed}");

            var maxPage = GetPaginationIfExists(html);
            var limit = Math.Min(maxPage, depth);

            news.AddRange(GetNewsIfExists(html));
            news.AddRange(await GetNewsFromPagination(termParsed, limit));

            return BuildResponseData(news);
        }

        private List<Noticia> BuildResponseData(List<string> news)
        {
            List<Noticia> formattedNews = [];
            foreach (var item in news)
            {
                var rawDate = _parser.QueryText(item, "span[class*='created-at']::text()");
                DateTime.TryParseExact(rawDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);
                formattedNews.Add(
                    new Noticia(
                        title: _parser.QueryText(item, "h5 > a::text()") ?? "No title",
                        dateTime: date,
                        year: date.Year,
                        link: _parser.QueryText(item, "h5 > a::attr(href)") ?? "No title",
                        @abstract: _parser.QueryText(item, "div[class*='news-text'] > p::text()"),
                        source: Name
                    )
                );
            }
            return formattedNews;
        }

        private async Task<List<string>> GetNewsFromPagination(string term, int maxPage)
        {
            List<string> newsRaw = [];
            for (var i = 0; i <= maxPage; i++)
            {
                var html = await _fetcher.GetStringAsync($"{BaseUrl}/page/{i}/?s={term}");
                var news = GetNewsIfExists(html);
                if (news.Count > 0)
                    newsRaw.AddRange(news);
            }
            return newsRaw;
        }

        private int GetPaginationIfExists(string document)
        {
            var pagination = _parser.QueryTexts(document, "ul[class*='pagination'] > li").ToList();
            var parsed = int.TryParse(pagination[^3], out int maxPage);

            _logger.LogInformation("Found {PageQuantity} pages to scrape", parsed);

            return parsed ? maxPage : 1;
        }

        private List<string> GetNewsIfExists(string document)
        {
            var noticias = _parser.QueryTexts(document, "div[id*='latestNews'] > div[class*='row']").ToList();

            _logger.LogInformation("Found {NewsCount} news", noticias.Count);

            return noticias;
        }
    }
}
