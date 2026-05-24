using ArgosSharp.Api.DTOs.Job.CreateJob;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Api.Mappers.JobMapper
{
    public interface IJobMapper
    {
        Job JobFromRequest(CreateJobRequest createJobRequest);
    }
}
