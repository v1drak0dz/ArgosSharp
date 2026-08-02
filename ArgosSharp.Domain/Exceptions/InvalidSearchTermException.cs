namespace ArgosSharp.Domain.Exceptions
{
    public class InvalidSearchTermException : Exception
    {
        public InvalidSearchTermException() : base("A invalid SearchTerm property was provided. SearchTerm must not be null or empty") { }
    }
}
