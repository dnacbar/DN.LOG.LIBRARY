using Microsoft.AspNetCore.Http;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

public abstract class BaseMiddleware(RequestDelegate requestDelegate)
{
    protected readonly RequestDelegate _requestDelegate = requestDelegate ?? throw new ArgumentNullException(nameof(requestDelegate));

    public abstract Task InvokeAsync(HttpContext httpContext);
}
