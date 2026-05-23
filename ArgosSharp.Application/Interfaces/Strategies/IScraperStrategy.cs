using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Interfaces.Strategies
{
    public interface IScraperStrategy
    {
        string Name { get; }

        Task<List<Noticia>> ProcessScraperAsync(string searchTerm, int depth);
    }
}
