namespace ArgosSharp.Application.Interfaces.Parser
{
    public interface IHtmlParser
    {
        string? QueryText(string html, string selector);

        IEnumerable<string> QueryTexts(string html, string selector);
    }
}
