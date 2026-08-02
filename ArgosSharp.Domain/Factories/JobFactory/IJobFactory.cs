using ArgosSharp.Domain.Model;

namespace ArgosSharp.Domain.Factories.JobFactory
{
    public interface IJobFactory
    {
        Job Create(string searchTerm, List<string> sites, int depth);
    }
}
