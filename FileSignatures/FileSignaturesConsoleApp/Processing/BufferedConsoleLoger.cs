namespace FileSignaturesConsoleApp
{
    public class BufferedConsoleLoger : IBufferedLogger
    {
        private const int _defaultLogBufferSize = 1000;

        private readonly int _bufferSize;

        private List<string> MessagesBuffer { get; set; }

        public BufferedConsoleLoger(int? bufferSize)
        {
            _bufferSize = bufferSize ?? _defaultLogBufferSize;
            MessagesBuffer = new List<string>(_bufferSize);
        }

        public void LogError(string message, Exception? ex = null)
        {
            string errorMessage = ex is null ? string.Empty : $"Message: {ex.Message}\nStackTrace: {ex.StackTrace}";
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
