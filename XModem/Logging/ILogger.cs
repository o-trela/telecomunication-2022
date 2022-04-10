namespace XModem.Logging;

public interface ILogger
{
    void Log(object message);
    void LogWarning(object message);
    void LogError(object message);
    void LogProgress(object message);
    void LogSuccess(object message);
}