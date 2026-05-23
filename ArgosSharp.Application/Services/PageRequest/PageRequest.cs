using System.Net.Http;

namespace ArgosSharp.Application.Services.PageRequest
{
    public class PageRequest : IPageRequest
    {
        private readonly HttpClient HttpClient = new();

        public async Task<string> GetPageAsync(string url)
        {
            try
            {
                var html = await HttpClient.GetStringAsync(url);
                return html;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
