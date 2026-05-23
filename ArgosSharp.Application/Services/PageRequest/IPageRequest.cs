namespace ArgosSharp.Application.Services.PageRequest
{
    public interface IPageRequest
    {
        Task<string> GetPageAsync(string url);
    }
}
