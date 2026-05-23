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
        public string Name => "caraguatatuba";

        public async Task<List<Noticia>> ProcessScraperAsync(string searchTerm, int depth)
        {
            var noticiasRaw = new List<string>();
            var noticias = new List<Noticia>();

            var termParsed = WebUtility.UrlEncode(searchTerm);

            _logger.LogInformation($"Initianting data gettering using term {searchTerm} in Caraguatatuba");

            var html = await _fetcher.GetStringAsync($"https://www.caraguatatuba.sp.gov.br/pmc/?s={termParsed}");

            var maxPage = GetPaginationIfExists(html);
            var limit = Math.Min(maxPage, depth);

            noticiasRaw.AddRange(GetNewsIfExists(html));

            for (var i = 2; i <= limit; i++)
            {
                var _html = await _fetcher.GetStringAsync($"https://www.caraguatatuba.sp.gov.br/pmc/page/{i}/?s={termParsed}");
                var news = GetNewsIfExists(_html);
                if (news.Count > 0)
                    noticiasRaw.AddRange(news);
            }

            foreach (var item in noticiasRaw)
            {
                var rawDate = _parser.QueryText(item, "span[class*='created-at']::text()");
                DateTime.TryParseExact(rawDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);
                noticias.Add(
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

            return noticias;
        }

        private int GetPaginationIfExists(string document)
        {
            var pagination = _parser.QueryTexts(document, "ul[class*='pagination'] > li").ToList();
            var parsed = int.TryParse(pagination[^3], out int maxPage);

            _logger.LogInformation($"Found {parsed} pages to scrape");

            return parsed ? maxPage : 1;
        }

        private List<string> GetNewsIfExists(string document)
        {
            var noticias = _parser.QueryTexts(document, "div[id*='latestNews'] > div[class*='row']").ToList();

            _logger.LogInformation($"Found {noticias.Count} news");

            return noticias;
        }
    }
}
