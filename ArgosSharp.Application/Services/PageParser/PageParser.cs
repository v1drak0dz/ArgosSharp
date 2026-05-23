using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace ArgosSharp.Application.Services.PageParser
{
    public class PageParser : IPageParser
    {
        private readonly HtmlParser HtmlParser = new();

        public async Task<IHtmlDocument> GetParsedDocumentAsync(string page)
        {
            try
            {
                var document = await HtmlParser.ParseDocumentAsync(page);
                return document;
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing document");
            }
        }
    }
}
