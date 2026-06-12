using AngleSharp.Browser;
using ArgosSharp.Application.Interfaces.Parser;
using Microsoft.Extensions.Logging;

namespace ArgosSharp.Infrastructure.Utils
{
    public static class PaginationExtractor
    {
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
