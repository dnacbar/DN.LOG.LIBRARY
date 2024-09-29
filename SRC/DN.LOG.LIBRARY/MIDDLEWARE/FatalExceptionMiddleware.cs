using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using DN.LOG.LIBRARY.MODEL.ENUM;
using System.Net;
using DN.LOG.LIBRARY.MODEL;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

public sealed class FatalExceptionMiddleware(RequestDelegate requestDelegate) : BaseMiddleware(requestDelegate)
{
    public override async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            //throw new Exception("Internal Server Error!");
            await _requestDelegate(httpContext);
        }
        catch (Exception ex)
        {
            LogExtension.CreateLog(new LogObject(
            Guid.NewGuid().ToString(),
            "DN",
            ex.ToString(),
            EnumLogLevel.Fatal,
            DateTime.Now,
            httpContext.Connection.RemoteIpAddress,
            HttpStatusCode.InternalServerError));

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
