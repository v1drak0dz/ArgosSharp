namespace ArgosSharp.Domain.Exceptions
{
    public class InvalidDepthException : Exception
    {
        public InvalidDepthException() : base("A invalid Depth property was provided. Depth must be greater than zero.") { }
    }
}
