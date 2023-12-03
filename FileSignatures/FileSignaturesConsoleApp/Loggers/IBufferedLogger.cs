namespace FileSignaturesConsoleApp
{
    public interface IBufferedLogger
    {
        void LogInfo(string message);

        void LogError(string message, Exception? ex = null);

        void LogImportant(string message);

        void WriteBufferedLogs();
    }
}
