using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Interfaces.Strategies
{
    public interface IScraperStrategy
    {
        string Name { get; set; }

        Task<List<Noticia>> ProcessScraperAsync(string searchTerm, int depth);
    }
}
