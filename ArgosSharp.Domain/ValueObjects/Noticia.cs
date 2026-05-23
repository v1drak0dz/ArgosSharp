namespace ArgosSharp.Domain.ValueObjects
{
    public class Noticia
    {
        public string Title { get; private set; } = string.Empty;
        public DateTime? DateTime { get; private set; }
        public int Year { get; private set; }
        public string Link { get; private set; } = string.Empty;
        public string Abstract { get; private set; } = string.Empty;
        public string Source { get; private set; } = string.Empty;

        public Noticia(
            string title,
            DateTime? dateTime,
            int year,
            string link,
            string? @abstract,
            string source
        )
        {
            Title = title;
            DateTime = dateTime;
            Year = year;
            Link = link;
            Source = source;

            if (!string.IsNullOrWhiteSpace(@abstract))
            {
                Abstract = @abstract;
            }
        }
    }
}