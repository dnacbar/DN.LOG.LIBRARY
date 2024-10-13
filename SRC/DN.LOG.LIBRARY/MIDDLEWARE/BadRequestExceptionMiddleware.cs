using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using DN.LOG.LIBRARY.MODEL.ENUM;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using System.Net;
using DN.LOG.LIBRARY.MODEL;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

internal sealed class BadRequestExceptionMiddleware(RequestDelegate requestDelegate) : BaseMiddleware(requestDelegate)
{
    protected override async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _requestDelegate(httpContext);
        }
        catch (BadRequestException ex)
        {
            ex.CreateLog(EnumLogLevel.Warning, httpContext.Connection.RemoteIpAddress, HttpStatusCode.BadRequest);

            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            await httpContext.Response.WriteAsync(string.Empty);
        }
    }
}

public static class BadRequestExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseBadRequestExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<BadRequestExceptionMiddleware>();
    }
}