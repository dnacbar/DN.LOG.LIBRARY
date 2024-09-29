using DN.LOG.LIBRARY.MODEL.ENUM;
using System.Net;

namespace DN.LOG.LIBRARY.MODEL;

public sealed class LogObject
{
    public LogObject(string id, string userLog, string infoLog, EnumLogLevel levelLog, DateTime timeLog, IPAddress iPAddress, HttpStatusCode httpStatusCode)
    {
        Id = id;
        UserLog = userLog;
        InfoLog = infoLog;
        LevelLog = levelLog;
        TimeLog = timeLog;
        IPAddress = iPAddress;
        HttpStatusCode = httpStatusCode;
    }

    public string Id { get; }
    public string UserLog { get; }
    public string InfoLog { get; }
    public EnumLogLevel LevelLog { get; }
    public DateTime TimeLog { get; }
    public IPAddress IPAddress { get; }
    public HttpStatusCode HttpStatusCode { get; }

    public string LogMessage()
    {
        return @"ID: " + Id + " DATETIME: " + TimeLog + " USER: " + UserLog + Environment.NewLine +
                "IP: " + IPAddress?.MapToIPv4().ToString() + Environment.NewLine +
                "LOG: " + InfoLog + Environment.NewLine + Environment.NewLine;
    }
}
