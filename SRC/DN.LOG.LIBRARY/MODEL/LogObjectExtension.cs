using DN.LOG.LIBRARY.MODEL.ENUM;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Net;
using System.Reflection;

namespace DN.LOG.LIBRARY.MODEL;

public static class LogObjectExtension
{
    public static void ConfigureSerilog()
    {
        var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name?.ToLower().Replace(".", "-");

        Log.Logger = new LoggerConfiguration()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", assemblyName)
            .WriteTo.Console()
            .WriteTo.Elasticsearch([new Uri("http://localhost:9200")], opts =>
            {
                opts.DataStream = new DataStreamName("LOG", "DN.LOG.LIBRARY", assemblyName);
                opts.BootstrapMethod = BootstrapMethod.Failure;
                opts.ConfigureChannel = channelOpts =>
                {
                    channelOpts.BufferOptions = new BufferOptions();
                };
            })
            .CreateLogger();
    }

    public static void CreateLog(this Exception ex, Microsoft.Extensions.Logging.ILogger logger, EnumLogLevel levelLog, IPAddress iPAddress = null)
    {
        var logMessage = iPAddress == null ? $"ID: {Guid.NewGuid()} DATETIME: {DateTime.Now}{Environment.NewLine}LOG: {ex}" :
            $"ID: {Guid.NewGuid()} DATETIME: {DateTime.Now} IP: {iPAddress?.MapToIPv4().ToString()}{Environment.NewLine}LOG: {ex}";

        switch (levelLog)
        {
            case EnumLogLevel.Critical:
                logger.LogCritical(logMessage);
                break;
            case EnumLogLevel.Error:
                logger.LogError(logMessage);
                break;
            case EnumLogLevel.Warning:
                logger.LogWarning(logMessage);
                break;
            case EnumLogLevel.Information:
                logger.LogInformation(logMessage);
                break;
            default:
                logger.LogTrace(logMessage);
                break;
        }
    }
}