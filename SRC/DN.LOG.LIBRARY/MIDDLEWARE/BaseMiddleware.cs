using Microsoft.AspNetCore.Http;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

internal abstract class BaseMiddleware(RequestDelegate requestDelegate)
{
    protected readonly RequestDelegate _requestDelegate = requestDelegate ?? throw new ArgumentNullException(nameof(requestDelegate));
    protected abstract Task InvokeAsync(HttpContext httpContext);
}
