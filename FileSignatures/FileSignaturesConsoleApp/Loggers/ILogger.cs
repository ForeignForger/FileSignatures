namespace FileSignaturesConsoleApp.Loggers
{
    public interface ILogger
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message, Exception? ex = null);
    }
}
