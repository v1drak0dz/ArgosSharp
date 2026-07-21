using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Domain.Factories.JobFactory
{
    public interface IJobFactory
    {
        Job Create(string searchTerm, List<string> sites, int depth);
    }
}
