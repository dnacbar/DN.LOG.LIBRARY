using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using DN.LOG.LIBRARY.MODEL.ENUM;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using System.Net;
using DN.LOG.LIBRARY.MODEL;
using Microsoft.Extensions.Logging;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

internal sealed class NotFoundExceptionMiddleware(ILogger<NotFoundExceptionMiddleware> logger, RequestDelegate requestDelegate) : BaseMiddleware(logger, requestDelegate)
{
    protected override async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _requestDelegate(httpContext);
        }
        catch (NotFoundException ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Warning, httpContext.Connection.RemoteIpAddress);

            httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

            await httpContext.Response.WriteAsync(string.Empty);
        }
    }
}

public static class NotFoundExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseNotFoundExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<NotFoundExceptionMiddleware>();
    }
}