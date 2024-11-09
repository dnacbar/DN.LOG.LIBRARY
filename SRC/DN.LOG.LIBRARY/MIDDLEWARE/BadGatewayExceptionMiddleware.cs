using DN.LOG.LIBRARY.MODEL;
using DN.LOG.LIBRARY.MODEL.ENUM;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

internal sealed class BadGatewayExceptionMiddleware(ILogger logger, RequestDelegate requestDelegate) : BaseMiddleware(logger, requestDelegate)
{
    public override async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _requestDelegate(httpContext);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            ex.CreateLog(_logger, EnumLogLevel.Error, httpContext.Connection.RemoteIpAddress);

            httpContext.Response.StatusCode = (int)HttpStatusCode.GatewayTimeout;

            await httpContext.Response.WriteAsync(string.Empty);
        }
        catch (HttpRequestException ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Error, httpContext.Connection.RemoteIpAddress);

            httpContext.Response.StatusCode = (int)HttpStatusCode.BadGateway;

            await httpContext.Response.WriteAsync(string.Empty);
        }
    }
}

public static class BadGatewayExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseBadGatewayExceptionMiddleware(this IApplicationBuilder builder, ILogger logger)
    {
        return builder.UseMiddleware<BadGatewayExceptionMiddleware>(logger);
    }
}
