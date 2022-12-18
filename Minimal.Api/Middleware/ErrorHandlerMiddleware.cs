using Minimal.Api.Exceptions;

namespace Minimal.Api.Middleware;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            switch (error)
            {
                case NotFoundException e:
                    // not found error
                    response.StatusCode = StatusCodes.Status404NotFound;
                    await response.WriteAsJsonAsync(new { message = e?.Message });
                    break;
                default:
                    // unhandled error
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    await response.WriteAsJsonAsync(new { message = error?.Message });
                    break;
            }

        }
    }
}
