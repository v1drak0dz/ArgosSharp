namespace ArgosSharp.Application.Interfaces.Fetcher
{
    public interface IHttpFetcher
    {
        /// <summary>
        /// Fetches the content of the specified URL as a string using an HTTP GET request.
        /// Throws an exception if the response status code indicates an error.
        /// </summary>
        /// <param name="url">The URL to fetch.</param>
        /// <returns>The response content as a string.</returns>
        /// <exception cref="HttpRequestException">Thrown when the HTTP response status code indicates an error.</exception>
        Task<string> GetStringAsync(string url);
    }
}
