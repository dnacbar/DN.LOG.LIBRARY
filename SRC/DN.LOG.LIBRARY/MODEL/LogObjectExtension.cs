using DN.LOG.LIBRARY.MODEL.ENUM;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Net;
using System.Reflection;

namespace DN.LOG.LIBRARY.MODEL;

public static class LogObjectExtension
{
    public static void ConfigureSerilog(ElasticSearch elasticSearch, IConfiguration configuration)
    {
        var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name?.ToLower().Replace(".", "-");

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Application", assemblyName)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Elasticsearch([new Uri(elasticSearch.Uri)], opts =>
            {
                opts.DataStream = new DataStreamName("LOG", "DN.LOG.LIBRARY", assemblyName);
                opts.BootstrapMethod = BootstrapMethod.Failure;
                opts.ConfigureChannel = channelOpts =>
                {
                    channelOpts.BufferOptions = new BufferOptions();
                };
            }, transport =>
            {
                if (!string.IsNullOrEmpty(elasticSearch.Username))
                    transport.Authentication(new BasicAuthentication(elasticSearch.Username, elasticSearch.Password));

                if (!string.IsNullOrEmpty(elasticSearch.ApiKey))
                    transport.Authentication(new ApiKey(elasticSearch.ApiKey));
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

public class ElasticSearch
{
    public string Uri { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ApiKey { get; set; }
}