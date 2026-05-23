namespace ArgosSharp.Application.Interfaces.Fetcher
{
    public interface IHttpFetcher
    {
        Task<string> GetStringAsync(string url);
    }
}
