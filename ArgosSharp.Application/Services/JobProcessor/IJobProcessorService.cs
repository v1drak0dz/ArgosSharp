using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Services.JobProcessor
{
    public interface IJobProcessorService
    {
        Task ProcessJobAsync(Job job);
    }
}
