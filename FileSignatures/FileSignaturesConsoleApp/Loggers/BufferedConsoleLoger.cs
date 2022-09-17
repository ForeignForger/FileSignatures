namespace FileSignaturesConsoleApp.Loggers
{
    public class BufferedConsoleLoger : IBufferedLogger
    {
        private readonly int _bufferSize;

        private List<string> MessagesBuffer { get; set; }

        public BufferedConsoleLoger(int bufferSize)
        {
            MessagesBuffer = new List<string>(bufferSize);
            _bufferSize = bufferSize;
        }

        public void LogError(string message, Exception? ex = null)
        {
            var errorMessage = ex is null ? string.Empty : $"Message: {ex.Message}\nStackTrace: {ex.StackTrace}";
            Console.WriteLine($"Error: {message}\n{errorMessage}");
        }

        public void LogImportant(string message)
        {
            Console.WriteLine($"{message}");
        }

        public void LogInfo(string message)
        {
            lock (MessagesBuffer)
            {
                MessagesBuffer.Add(message);

                if (MessagesBuffer.Count >= _bufferSize)
                {
                    WriteBufferedLogs();
                }
            }
        }

        public void WriteBufferedLogs()
        {
            lock (MessagesBuffer)
            {
                if (MessagesBuffer.Any())
                {
                    Console.WriteLine(string.Join('\n', MessagesBuffer));
                    MessagesBuffer = new List<string>(_bufferSize);
                }
            }
        }
    }
}
