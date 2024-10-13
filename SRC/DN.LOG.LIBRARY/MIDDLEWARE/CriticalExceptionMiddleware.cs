using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using DN.LOG.LIBRARY.MODEL.ENUM;
using System.Net;
using DN.LOG.LIBRARY.MODEL;
using Microsoft.Extensions.Logging;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

internal sealed class CriticalExceptionMiddleware(ILogger<CriticalExceptionMiddleware> logger, RequestDelegate requestDelegate) : BaseMiddleware(logger, requestDelegate)
{
    protected override async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _requestDelegate(httpContext);
        }
        catch (Exception ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Critical, httpContext.Connection.RemoteIpAddress);

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await httpContext.Response.WriteAsync(string.Empty);
        }
    }
}

public static class CriticalExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseFatalExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CriticalExceptionMiddleware>();
    }
}
