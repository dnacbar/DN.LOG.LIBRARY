using DN.LOG.LIBRARY.MODEL;
using DN.LOG.LIBRARY.MODEL.ENUM;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

public class BadGatewayExceptionMiddleware(RequestDelegate requestDelegate) : BaseMiddleware(requestDelegate)
{
    public override async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _requestDelegate(httpContext);
        }
        catch (TaskCanceledException ex) when(ex.InnerException is TimeoutException)
        {
            LogExtension.CreateLog(new LogObject(
            Guid.NewGuid().ToString(),
            "DN",
            ex.ToString(),
            EnumLogLevel.Error,
            DateTime.Now,
            httpContext.Connection.RemoteIpAddress,
            HttpStatusCode.GatewayTimeout));

            httpContext.Response.StatusCode = (int)HttpStatusCode.GatewayTimeout;

            await httpContext.Response.WriteAsync(string.Empty);
        }
        catch (HttpRequestException ex)
        {
            LogExtension.CreateLog(new LogObject(
            Guid.NewGuid().ToString(),
            "DN",
            ex.ToString(),
            EnumLogLevel.Error,
            DateTime.Now,
            httpContext.Connection.RemoteIpAddress,
            HttpStatusCode.BadGateway));

            httpContext.Response.StatusCode = (int)HttpStatusCode.BadGateway;

            await httpContext.Response.WriteAsync(string.Empty);
        }
    }
}

public static class BadGatewayExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseBadGatewayExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<BadGatewayExceptionMiddleware>();
    }
}
