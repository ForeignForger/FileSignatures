namespace FileSignaturesConsoleApp.Entities
{
    public class Settings
    {
        public String FilePath { get; set; }

        public Int32 SegmentSize { get; set; }

        public Int32? LogBufferSize { get; set; }

        public Int32 WorkThreadsCount { get; set; }

        public QueueSettings QueueSettings { get; set; }

        public Boolean DoCountDown { get; set; }

        public Settings() 
        {
            QueueSettings = new QueueSettings();
            LogBufferSize = null;
            DoCountDown = false;
        }
    }
}
