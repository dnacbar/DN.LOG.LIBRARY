using DN.LOG.LIBRARY.MODEL;
using DN.LOG.LIBRARY.MODEL.ENUM;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using System.Net;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

public class DataBaseExceptionMiddleware(RequestDelegate requestDelegate) : BaseMiddleware(requestDelegate)
{
    private const int SQL_TIMEOUT = -2;

    public override async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _requestDelegate(httpContext);
        }
        catch (SqlException ex)
        {
            HttpStatusCode httpStatusCode;

            if (ex.Number == SQL_TIMEOUT)
                httpStatusCode = HttpStatusCode.RequestTimeout;
            else 
                httpStatusCode = HttpStatusCode.BadRequest;

            LogExtension.CreateLog(new LogObject(
            Guid.NewGuid().ToString(),
            "DN",
            ex.ToString(),
            EnumLogLevel.Error,
            DateTime.Now,
            httpContext.Connection.RemoteIpAddress,
            httpStatusCode));

            httpContext.Response.StatusCode = (int)httpStatusCode;

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
