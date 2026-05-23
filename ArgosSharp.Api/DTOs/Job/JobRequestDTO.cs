namespace ArgosSharp.Api.DTOs.Job
{
    public class JobRequestDTO
    {
        public string searchTerm { get; set; }
        public Dictionary<string, object> parameters { get; set; }
    }
}
