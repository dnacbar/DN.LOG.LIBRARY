using DN.LOG.LIBRARY.MIDDLEWARE;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.DependencyInjection;

namespace DN.LOG.LIBRARY.TEST.MIDDLEWARE;

public class MiddlewareTest
{
    [Fact]
    public async Task TestCriticalMiddleware()
    {
        using var host = await new HostBuilder()
        .ConfigureWebHost(webBuilder =>
        {
            webBuilder
                .UseTestServer()
                .ConfigureServices(x => { })
                .Configure(app =>
                {
                    app.UseCriticalExceptionMiddleware(LoggerFactory.Create(x => { }).CreateLogger(""));
                    app.Run(context => throw new Exception("TESTE QUE DEU MUITO ERRADO!"));
                });
        })
        .StartAsync();

        var response = host.GetTestClient().GetAsync("/");

        Assert.Equal(HttpStatusCode.InternalServerError, (await response).StatusCode);
    }

    [Fact]
    public async Task TestValidationMiddleware()
    {
        using var host = await new HostBuilder()
        .ConfigureWebHost(webBuilder =>
        {
            webBuilder
                .UseTestServer()
                .ConfigureServices(x => { })
                .Configure(app =>
                {
                    app.UseBadRequestExceptionMiddleware(LoggerFactory.Create(x => { }).CreateLogger(""));
                    app.Run(context => throw new BadRequestException("ERRO AO EXECUTAR A CHAMADA HTTP!"));
                });
        })
        .StartAsync();

        var response = host.GetTestClient().GetAsync("/");

        Assert.Equal(HttpStatusCode.BadRequest, (await response).StatusCode);
    }

    [Fact]
    public async Task TestDataBaseMiddleware()
    {
        var sqlExceptionCtor = typeof(SqlException)
        .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
        .FirstOrDefault(c => c.GetParameters().Length == 4);

        var sqlException = sqlExceptionCtor.Invoke(
        [
            "Simulated SQL Exception",  // Message
            null,                       // SqlErrorCollection
            null,                       // Server (null)
            Guid.NewGuid()              // ClientConnectionId
        ]) as SqlException;

        using var host = await new HostBuilder()
        .ConfigureWebHost(webBuilder =>
        {
            webBuilder
                .UseTestServer()
                .ConfigureServices(x => { })
                .Configure(app =>
                {
                    app.UseDataBaseExceptionMiddleware(LoggerFactory.Create(x => { }).CreateLogger(""));
                    app.Run(context => throw sqlException);
                });
        })
        .StartAsync();

        var response = host.GetTestClient().GetAsync("/");

        Assert.Equal(HttpStatusCode.BadRequest, (await response).StatusCode);
    }

    [Fact]
    public async Task TestBadGatewayMiddleware()
    {
        using var host = await new HostBuilder()
        .ConfigureWebHost(webBuilder =>
        {
            webBuilder
                .UseTestServer()
                .ConfigureServices(x => { })
                .Configure(app =>
                {
                    app.UseBadGatewayExceptionMiddleware(LoggerFactory.Create(x => { }).CreateLogger(""));
                    app.Run(context => throw new HttpRequestException("ERRO AO EXECUTAR A CHAMADA HTTP!"));
                });
        })
        .StartAsync();

        var response = host.GetTestClient().GetAsync("/");

        Assert.Equal(HttpStatusCode.BadGateway, (await response).StatusCode);
    }

    [Fact]
    public async Task TestNotFoundMiddleware()
    {
        using var host = await new HostBuilder()
        .ConfigureWebHost(webBuilder =>
        {
            webBuilder
                .UseTestServer()
                .ConfigureServices(x => { })
                .Configure(app =>
                {
                    app.UseNotFoundExceptionMiddleware(LoggerFactory.Create(x => { }).CreateLogger(""));
                    app.Run(context => throw new NotFoundException("REGISTRO NÃO ENCONTRADO!"));
                });
        })
        .StartAsync();

        var response = host.GetTestClient().GetAsync("/");

        Assert.Equal(HttpStatusCode.NotFound, (await response).StatusCode);
    }
}