using ArgosSharp.Api.DTOs.Job.CreateJob;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.Factories.JobFactory;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Api.Mappers.JobMapper
{
    public class JobMapper(IJobFactory jobFactory)
    {
        public Job JobFromRequest(CreateJobRequest createJobRequest)
        {
            var sites = new List<ScraperSourceEnum>();
            
            foreach (var site in createJobRequest.Parameters.Sites)
            {
                switch (site)
                {
                    case "caraguatatuba":
                        sites.Add(ScraperSourceEnum.Caraguatatuba);
                        break;
                    case "ubatuba":
                        sites.Add(ScraperSourceEnum.Ubatuba);
                        break;
                    case "saosebastiao":
                        sites.Add(ScraperSourceEnum.SaoSebastiao);
                        break;
                    default:
                        throw new ArgumentException($"Source {site} not found!");
                }
            }

            var parameters = new JobParameters(sites, createJobRequest.Parameters.Depth);
            
            var job = jobFactory.Create(createJobRequest.SearchTerm, parameters);

            return job;
        }
    }
}
