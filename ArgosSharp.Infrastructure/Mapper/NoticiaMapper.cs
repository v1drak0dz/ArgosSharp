using ArgosSharp.Application.Interfaces.Parser;
using ArgosSharp.Domain.ValueObjects;
using System.Globalization;
using System.Xml.Linq;

namespace ArgosSharp.Infrastructure.Mapper
{
    public static class NoticiaMapper
    {
        public static Noticia Map(
            string html,
            IHtmlParser parser,
            ScraperSelectors selectors,
            string source
            )
        {
            var rawDate = parser.QueryText(html, selectors.Date);
            DateTime.TryParseExact(rawDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);
            return new Noticia(
                title: parser.QueryText(html, selectors.Title) ?? "No title",
                dateTime: date,
                year: date.Year,
                link: parser.QueryText(html, selectors.Link) ?? "No link",
                @abstract: parser.QueryText(html, selectors.Abstract) ?? "",
                source: source
            );
        }
    }
}
