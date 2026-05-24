namespace ArgosSharp.Api.DTOs.Job.CreateJob
{
    public class CreateJobParametersRequest
    {
        public List<string> Sites { get; set; } = [];
        public int Depth { get; set; } = 0;
    }
}
