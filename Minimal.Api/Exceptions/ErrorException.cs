namespace Minimal.Api.Exceptions;

public class ErrorException : Exception
{
    public ErrorException()
        : base("Error")
    {
    }

    public ErrorException(string message)
        : base(message)
    {
    }

    public ErrorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}