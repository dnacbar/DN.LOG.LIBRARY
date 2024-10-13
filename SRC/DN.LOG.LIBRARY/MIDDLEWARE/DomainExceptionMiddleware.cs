using DN.LOG.LIBRARY.MODEL.ENUM;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using DN.LOG.LIBRARY.MODEL;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

internal sealed class DomainExceptionMiddleware(RequestDelegate requestDelegate) : BaseMiddleware(requestDelegate)
{
    protected override async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _requestDelegate(httpContext);
        }
        catch (DomainException ex)
        {
            ex.CreateLog(EnumLogLevel.Error, httpContext.Connection.RemoteIpAddress, HttpStatusCode.BadRequest);

            httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;

            await httpContext.Response.WriteAsync(string.Empty);
        }
    }
}

public static class DomainExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseDomainExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<DomainExceptionMiddleware>();
    }
}