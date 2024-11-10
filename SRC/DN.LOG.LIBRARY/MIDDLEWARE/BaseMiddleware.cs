using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

internal abstract class BaseMiddleware(ILogger logger, RequestDelegate requestDelegate)
{
    protected readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    protected readonly RequestDelegate _requestDelegate = requestDelegate ?? throw new ArgumentNullException(nameof(requestDelegate));
    internal abstract Task InvokeAsync(HttpContext httpContext);
}
