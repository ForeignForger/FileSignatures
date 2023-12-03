namespace FileSignaturesConsoleApp
{
    public class Settings
    {
        public String FilePath { get; }
        public Int32 SegmentSize { get; }
        public Int32 WorkThreadsCount { get; }
        public QueueSettings QueueSettings { get; }
        public Int32? LogBufferSize { get; }
        public Boolean DoCountDown { get; }

        public Settings(String filePath, Int32 segmentSize, Int32 workThreadsCount, QueueSettings queueSettings, Int32? logBufferSize, Boolean doCountDown) 
        {
            FilePath = filePath;
            SegmentSize = segmentSize;
            WorkThreadsCount = workThreadsCount;
            QueueSettings = queueSettings;
            DoCountDown = doCountDown;
            LogBufferSize = logBufferSize;
        }
    }
}
