﻿using DN.LOG.LIBRARY.MODEL.ENUM;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using DN.LOG.LIBRARY.MODEL;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace DN.LOG.LIBRARY.MIDDLEWARE;

internal sealed class DomainExceptionMiddleware(ILogger logger, RequestDelegate requestDelegate) : BaseMiddleware(logger, requestDelegate)
{
    internal override async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _requestDelegate(httpContext);
        }
        catch (DomainException ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Error, httpContext.Connection.RemoteIpAddress);

            httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;

            await httpContext.Response.WriteAsync(string.Empty);
        }
    }
}

public static class DomainExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseDomainExceptionMiddleware(this IApplicationBuilder builder, ILogger logger)
    {
        return builder.UseMiddleware<DomainExceptionMiddleware>(logger);
    }
}