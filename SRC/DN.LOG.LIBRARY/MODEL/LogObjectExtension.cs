using DN.LOG.LIBRARY.MODEL.ENUM;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DN.LOG.LIBRARY.MODEL;

public static class LogObjectExtension
{
    public static void CreateLog(this Exception ex, ILogger logger, EnumLogLevel levelLog, IPAddress iPAddress = null)
    {
        var logMessage = iPAddress == null ?
            string.Concat(@"ID: {0} DATETIME: {1}{3}LOG: {4}", Guid.NewGuid(), DateTime.Now.ToString(), Environment.NewLine, ex.ToString()) :
            string.Concat(@"ID: {0} DATETIME: {1} IP: {2}{3}LOG: {4}", Guid.NewGuid(), DateTime.Now.ToString(), iPAddress?.MapToIPv4().ToString(), Environment.NewLine, ex.ToString());

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