namespace FileSignaturesConsoleApp
{
    public class ConsoleLogger : ILogger
    {
        public void LogInfo(string message)
        {
            Console.WriteLine($"{message}");
        }

        public void LogWarning(string message)
        {
            Console.WriteLine($"WARNING: {message}");
        }
        public void LogError(string message, Exception? ex = null)
        {
            var errorMessage = ex is null ? string.Empty : $"Error Message: {ex.Message}\nStackTrace: {ex.StackTrace}";
            Console.WriteLine($"{message}\n{errorMessage}");
        }
    }
}
