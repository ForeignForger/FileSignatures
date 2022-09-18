namespace FileSignaturesConsoleApp.Entities
{
    public class Settings
    {
        public string FilePath { get; set; }

        public int SegmentSize { get; set; }

        public int LogBufferSize { get; set; }

        public int WorkThreadsCount { get; set; }

        public QueueSettings QueueSettings { get; set; }
    }
}
