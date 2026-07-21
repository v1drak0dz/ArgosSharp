using ArgosSharp.Application.Interfaces.Parser;
using ArgosSharp.Domain.ValueObjects;
using System.Globalization;

namespace ArgosSharp.Infrastructure.Mapper
{
    public static class NoticiaMapper
    {
        /// <summary>
        /// Maps HTML content to a Noticia object by extracting relevant fields using the provided
        /// HTML parser and CSS selectors. Parses the date field in "dd/MM/yyyy" format and extracts
        /// title, date, link, abstract, and source. Missing title defaults to "No title" and
        /// missing link defaults to "No link".
        /// </summary>
        /// <param name="html">The HTML content to map.</param>
        /// <param name="parser">The HTML parser to use for extracting text content.</param>
        /// <param name="selectors">The CSS selectors configuration for extracting specific fields.</param>
        /// <param name="source">The source identifier to associate with the news item.</param>
        /// <returns>A new <see cref="Noticia"/> instance populated from the parsed HTML.</returns>
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
