namespace ArgosSharp.Api.DTOs.Job.CreateJob
{
    public class CreateJobRequest
    {
        public string SearchTerm { get; set; } = string.Empty;
        public CreateJobParametersRequest Parameters { get; set; } = new();
    }
}
