using DN.LOG.LIBRARY.MODEL.ENUM;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DN.LOG.LIBRARY.MODEL;

public static class LogObjectExtension
{
    public static void CreateLog(this Exception ex, ILogger logger, EnumLogLevel levelLog, IPAddress iPAddress = null)
    {
        var logMessage = @"ID: " + Guid.NewGuid() +
                        " DATETIME: " + DateTime.Now.ToString() +
                        " IP: " + iPAddress?.MapToIPv4().ToString() + Environment.NewLine +
                        "LOG: " + ex.ToString();

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