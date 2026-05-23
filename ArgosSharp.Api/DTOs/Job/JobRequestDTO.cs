namespace ArgosSharp.Api.DTOs.Job
{
    public class JobRequestDTO
    {
        public string searchTerm { get; set; }
        public int    depth      { get; set; }
        public Dictionary<string, object> parameters { get; set; }
    }
}
