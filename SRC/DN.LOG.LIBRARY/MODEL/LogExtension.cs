namespace DN.LOG.LIBRARY.MODEL;

public static class LogExtension
{
    public static void CreateLog(LogObject logObject)
    {
        File.AppendAllText(Path.Combine(Path.GetPathRoot(Directory.GetCurrentDirectory()), "log", "LOG_LEVEL_" + logObject.LevelLog.ToString().ToUpperInvariant() + "_" + (int)logObject.HttpStatusCode + "_" + DateTime.Today.ToString("yyyy_MM_dd") + ".txt"),
                           logObject.LogMessage(),
                           System.Text.Encoding.UTF8);
    }
}
