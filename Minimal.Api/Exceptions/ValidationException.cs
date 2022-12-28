using FluentValidation.Results;

namespace Minimal.Api.Exceptions;

public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string propertyName, string errorMessage)
        : this()
    {
        Errors = new Dictionary<string, string[]>()
        {
            { propertyName, new string[] { errorMessage } }
        };
    }

    public ValidationException(string propertyName, string[] errorMessage)
        : this()
    {
        Errors = new Dictionary<string, string[]>()
        {
            { propertyName, errorMessage }
        };
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }
}