using DN.LOG.LIBRARY.MODEL.ENUM;
using System.Net;

namespace DN.LOG.LIBRARY.MODEL;

public static class LogObjectExtension
{
    public static void CreateLog(this Exception ex, EnumLogLevel levelLog, IPAddress iPAddress = null, HttpStatusCode? httpStatusCode = null)
    {
        var logMessage = @"ID: " + Guid.NewGuid() +
                        " DATETIME: " + DateTime.Now.ToString() +
                        "IP: " + iPAddress?.MapToIPv4().ToString() + Environment.NewLine +
                        "LOG: " + ex.ToString() + Environment.NewLine + Environment.NewLine;

        Console.WriteLine(logMessage);
    }
}