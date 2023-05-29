enum LogLevel
{
  TRACE,
  DEBUG,
  INFO,
  LOG,
  WARNING,
  ERROR
}

public static class Logger
{
  private static string GetLevelName(LogLevel level)
  {
    switch (level)
    {
      case LogLevel.TRACE:
        return "TRACE";
      case LogLevel.DEBUG:
        return "DEBUG";
      case LogLevel.INFO:
        return "INFO";
      case LogLevel.LOG:
        return "LOG";
      case LogLevel.WARNING:
        return "WARNING";
      case LogLevel.ERROR:
        return "ERROR";
      default:
        return "UNKNOWN";
    }
  }

  private static string GetLogTimestamp()
  {
    return DateTime.Now.ToString("HH:mm:ss.fff");
  }

  private static string GetHeader(LogLevel level)
  {
    return $"[{GetLogTimestamp()}] [{GetLevelName(level)}]";
  }

  private static void _Log(LogLevel level, string message)
  {
    Print($"{GetHeader(level)} {message}");
  }

  public static void Trace(string message)
  {
    _Log(LogLevel.TRACE, message);
  }

  public static void Debug(string message)
  {
    _Log(LogLevel.DEBUG, message);
  }

  public static void Info(string message)
  {
    _Log(LogLevel.INFO, message);
  }

  public static void Log(string message)
  {
    _Log(LogLevel.LOG, message);
  }

  public static void Warning(string message)
  {
    _Log(LogLevel.WARNING, message);
  }

  public static void Error(string message)
  {
    _Log(LogLevel.ERROR, message);
  }
}
