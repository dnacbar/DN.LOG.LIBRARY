using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using DN.LOG.LIBRARY.MODEL.ENUM;
using System.Net;
using DN.LOG.LIBRARY.MODEL;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

internal sealed class FatalExceptionMiddleware(RequestDelegate requestDelegate) : BaseMiddleware(requestDelegate)
{
    protected override async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _requestDelegate(httpContext);
        }
        catch (Exception ex)
        {
            ex.CreateLog(EnumLogLevel.Fatal, httpContext.Connection.RemoteIpAddress, HttpStatusCode.InternalServerError);

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await httpContext.Response.WriteAsync(string.Empty);
        }
    }
}

public static class FatalExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseFatalExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<FatalExceptionMiddleware>();
    }
}
