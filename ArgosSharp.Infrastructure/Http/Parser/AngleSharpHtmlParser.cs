using AngleSharp.Html.Parser;
using ArgoSharp = ArgosSharp.Application.Interfaces.Parser;

namespace ArgosSharp.Infrastructure.Http.Parser
{
    public class AngleSharpHtmlParser : ArgoSharp.IHtmlParser
    {
        private readonly HtmlParser htmlParser = new();

        public string? QueryText(string html, string selector)
        {
            var document = htmlParser.ParseDocument(html);

            return document.QuerySelector(selector)?.TextContent.Trim();
        }

        public IEnumerable<string> QueryTexts(string html, string selector)
        {
            var document = htmlParser.ParseDocument(html);
            return document.QuerySelectorAll(selector).Select(x => x.TextContent.Trim());
        }
    }
}
