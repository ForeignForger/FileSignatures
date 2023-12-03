namespace FileSignaturesConsoleApp
{
    public class WorkerThread
    {
        public readonly Thread Thread;

        public readonly EventWaitHandle EventWaitHandle;

        public Segment? Segment { get; set; }

        public WorkerThread(Thread thread, EventWaitHandle eventWaitHandle)
        {      
            this.Thread = thread;
            this.EventWaitHandle = eventWaitHandle;
        }
    }
}
