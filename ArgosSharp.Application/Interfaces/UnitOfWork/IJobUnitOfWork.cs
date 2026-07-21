using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Interfaces.UnitOfWork
{
    public  interface IJobUnitOfWork
    {
        Task AddJobAsync(Job job);
    }
}
