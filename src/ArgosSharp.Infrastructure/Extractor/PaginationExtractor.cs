using AngleSharp.Browser;
using ArgosSharp.Application.Interfaces.Parser;
using Microsoft.Extensions.Logging;

namespace ArgosSharp.Infrastructure.Utils
{
    public static class PaginationExtractor
    {
        /// <summary>
        /// Extracts and returns the maximum page number from a collection of pagination strings.
        /// Parses each string to an integer, filters out invalid values, and returns the maximum.
        /// Returns 1 if the collection is null, empty, or contains no valid integers.
        /// </summary>
        /// <param name="pagination">A collection of page number strings to parse, or null.</param>
        /// <returns>The maximum page number found, or 1 if none exist.</returns>
        public static int GetMaxPage(IEnumerable<string>? pagination)
        {
            var numbers = pagination?
                .Select(x => int.TryParse(x.Trim(), out var n) ? n : (int?)null)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToList();

            return numbers!.Any() ? numbers!.Max() : 1;
        }
    }
}
