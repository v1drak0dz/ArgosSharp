namespace ArgosSharp.Domain.Exceptions
{
    public class InvalidSitesException : Exception
    {
        public InvalidSitesException() : base("Invalid Sites property was provided. Sites must contain at least one item.") { }
    }
}
