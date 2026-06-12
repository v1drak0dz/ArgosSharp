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
    [ScraperSourceAnnotation(ScraperSourceEnum.SaoSebastiao)]
    public class SaoSebastiaoScraper(ILogger<SaoSebastiaoScraper> _logger, IHttpFetcher _fetcher, IHtmlParser _parser) : IScraperStrategy
    {
        public string Name => "sao_sebastiao";

        private const string BaseUrl = "https://www.saosebastiao.sp.gov.br/";
        private const string BaseSearch = BaseUrl + "noticia-lista.asp?idTitulo=";
        private const string PaginationQuery = "div[id*='news_paging'] > li";
        private const string NewsQuery = "div[id*='page-content'] > article";
        private const string DateQuery = "div[class*='notice-date']::text()";
        private const string TitleQuery = "h2 > a::text()";
        private const string LinkQuery = "h2 > a::text()";

        public async Task<List<Noticia>> ProcessScraperAsync(string searchTerm, int depth)
        {
            var news = new List<string>();
            var termParsed = WebUtility.UrlEncode(searchTerm);

            _logger.LogInformation("Initianting data gettering using term {SearchTerm} in São Sebastião", searchTerm);

            var html = await _fetcher.GetStringAsync($"{BaseSearch}{termParsed}");
            var maxPage = GetPaginationIfExists(html);
            var limit = Math.Min(maxPage, depth);

            news.AddRange(GetNewsIfExists(html));
            news.AddRange(await GetNewsFromPagination(termParsed, limit));

            return BuildResponseData(news);
        }

        private int GetPaginationIfExists(string html)
        {
            var pagination = _parser.QueryTexts(html, PaginationQuery).ToList();
            var parsed = int.TryParse(pagination[^2], out int maxPage);
            _logger.LogInformation("Found {PageQuantity} pages to scrape", parsed);
            return parsed ? maxPage : 1;
        }

        private List<string> GetNewsIfExists(string document)
        {
            var noticias = _parser.QueryTexts(document, NewsQuery).ToList();
            _logger.LogInformation("Found {NewsCount} news", noticias.Count);
            return noticias;
        }

        private async Task<List<string>> GetNewsFromPagination(string term, int maxPage)
        {
            List<string> newsRaw = [];
            for (var i = 0; i <= maxPage; i++)
            {
                var html = await _fetcher.GetStringAsync($"{BaseSearch}{term}&pg={i}");
                var news = GetNewsIfExists(html);
                if (news.Count > 0)
                    newsRaw.AddRange(news);
            }
            return newsRaw;
        }

        private List<Noticia> BuildResponseData(List<string> news)
        {
            List<Noticia> formattedNews = [];
            foreach (var item in news)
            {
                var rawDate = _parser.QueryText(item, DateQuery);
                DateTime.TryParseExact(rawDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date); ;
                formattedNews.Add(
                    new Noticia(
                        title: _parser.QueryText(item, TitleQuery) ?? "No title",
                        dateTime: date,
                        year: date.Year,
                        link: _parser.QueryText(item, LinkQuery) ?? "No link",
                        @abstract: "",
                        source: Name
                    )
                );
            }
            return formattedNews;
        }
    }
}
