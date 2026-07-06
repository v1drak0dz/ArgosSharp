namespace ArgosSharp.Application.Interfaces.Parser
{
    public interface IHtmlParser
    {
        /// <summary>
        /// Parses the provided HTML and extracts the text content of the first element
        /// matching the specified CSS selector.
        /// </summary>
        /// <param name="html">The HTML content to parse.</param>
        /// <param name="selector">The CSS selector to match.</param>
        /// <returns>The trimmed text content of the matched element, or null if no element is found.</returns>
        string? QueryText(string html, string selector);

        /// <summary>
        /// Parses the provided HTML and extracts the text content of all elements
        /// matching the specified CSS selector.
        /// </summary>
        /// <param name="html">The HTML content to parse.</param>
        /// <param name="selector">The CSS selector to match.</param>
        /// <returns>An enumerable collection of trimmed text content from all matched elements.</returns>
        IEnumerable<string> QueryTexts(string html, string selector);
    }
}
