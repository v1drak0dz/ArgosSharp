using ArgosSharp.Application.Interfaces.Fetcher;

namespace ArgosSharp.Infrastructure.Http.Fetcher
{
    public class HttpClientFetcher(HttpClient httpClient) : IHttpFetcher
    {
        public async Task<string> GetStringAsync(string url)
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
