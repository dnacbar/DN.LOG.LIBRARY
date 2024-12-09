using DN.LOG.LIBRARY.MODEL;
using DN.LOG.LIBRARY.MODEL.ENUM;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Net;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

internal sealed class DataBaseExceptionMiddleware(ILogger logger, RequestDelegate requestDelegate) : BaseMiddleware(logger, requestDelegate)
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

            ex.CreateLog(_logger, EnumLogLevel.Error, httpContext.Connection.RemoteIpAddress);

            httpContext.Response.StatusCode = (int)httpStatusCode;

            await httpContext.Response.WriteAsync(string.Empty);
        }
    }
}

public static class DataBaseExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseDataBaseExceptionMiddleware(this IApplicationBuilder builder, ILogger logger)
    {
        return builder.UseMiddleware<DataBaseExceptionMiddleware>(logger);
    }
}
