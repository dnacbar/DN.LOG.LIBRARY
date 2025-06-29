using DN.LOG.LIBRARY.MODEL;
using DN.LOG.LIBRARY.MODEL.ENUM;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace DN.LOG.LIBRARY.TEST.MODEL;

public class LogObjectExtensionTest
{
    [Fact]
    public void CreateLog_ShouldLogCriticalMessage_WhenLevelIsCritical()
    {
        var loggerMock = new Mock<ILogger>();
        var ex = new Exception("Critical error");

        ex.CreateLog(loggerMock.Object, EnumLogLevel.Critical);
        Assert.True(loggerMock.Invocations.Count > 0);
    }
}
