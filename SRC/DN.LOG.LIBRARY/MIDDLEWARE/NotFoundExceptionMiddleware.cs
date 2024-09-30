using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using DN.LOG.LIBRARY.MODEL.ENUM;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using System.Net;
using DN.LOG.LIBRARY.MODEL;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

public class NotFoundExceptionMiddleware(RequestDelegate requestDelegate) : BaseMiddleware(requestDelegate)
{
    public override async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _requestDelegate(httpContext);
        }
        catch (NotFoundException ex)
        {
            LogExtension.CreateLog(new LogObject
            (Guid.NewGuid().ToString(), 
            "DN", 
            ex.ToString(), 
            EnumLogLevel.Warning, 
            DateTime.Now, 
            httpContext.Connection.RemoteIpAddress,
            HttpStatusCode.NotFound));

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