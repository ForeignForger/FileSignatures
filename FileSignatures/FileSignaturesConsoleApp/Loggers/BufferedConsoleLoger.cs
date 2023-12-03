namespace FileSignaturesConsoleApp
{
    public class BufferedConsoleLoger : IBufferedLogger
    {
        private const Int32 _defaultLogBufferSize = 1000;

        private readonly Int32 _bufferSize;

        private List<String> MessagesBuffer { get; set; }

        public BufferedConsoleLoger(Int32? bufferSize)
        {
            _bufferSize = bufferSize ?? _defaultLogBufferSize;
            MessagesBuffer = new List<String>(_bufferSize);
        }

        public void LogError(String message, Exception? ex = null)
        {
            String errorMessage = ex is null ? String.Empty : $"Message: {ex.Message}\nStackTrace: {ex.StackTrace}";
            Console.WriteLine($"Error: {message}\n{errorMessage}");
        }

        public void LogImportant(String message)
        {
            Console.WriteLine($"{message}");
        }

        public void LogInfo(String message)
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
                    Console.WriteLine(String.Join('\n', MessagesBuffer));
                    MessagesBuffer = new List<String>(_bufferSize);
                }
            }
        }
    }
}
