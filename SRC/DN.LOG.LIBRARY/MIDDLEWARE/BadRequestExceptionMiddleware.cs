using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using DN.LOG.LIBRARY.MODEL.ENUM;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using System.Net;
using DN.LOG.LIBRARY.MODEL;
using Microsoft.Extensions.Logging;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

internal sealed class BadRequestExceptionMiddleware(ILogger logger, RequestDelegate requestDelegate) : BaseMiddleware(logger, requestDelegate)
{
    public override async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _requestDelegate(httpContext);
        }
        catch (BadRequestException ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Warning, httpContext.Connection.RemoteIpAddress);

            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            await httpContext.Response.WriteAsync(string.Empty);
        }
    }
}

public static class BadRequestExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseBadRequestExceptionMiddleware(this IApplicationBuilder builder, ILogger logger)
    {
        return builder.UseMiddleware<BadRequestExceptionMiddleware>(logger);
    }
}