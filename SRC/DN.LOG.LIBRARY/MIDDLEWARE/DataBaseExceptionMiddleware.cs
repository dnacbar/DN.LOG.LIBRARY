using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using DN.LOG.LIBRARY.MODEL.ENUM;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using System.Net;
using DN.LOG.LIBRARY.MODEL;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

public class DataBaseExceptionMiddleware(RequestDelegate requestDelegate) : BaseMiddleware(requestDelegate)
{
    public override async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _requestDelegate(httpContext);
        }
        catch (DataBaseException ex)
        {
            LogExtension.CreateLog(new LogObject(
            Guid.NewGuid().ToString(),
            "DN",
            ex.ToString(),
            EnumLogLevel.Error,
            DateTime.Now,
            httpContext.Connection.RemoteIpAddress,
            HttpStatusCode.BadRequest));

            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            await httpContext.Response.WriteAsync(string.Empty);
        }
    }
}

public static class DataBaseExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseDataBaseExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<DataBaseExceptionMiddleware>();
    }
}
