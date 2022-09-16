using FileSignaturesConsoleApp.FileReaders;
using FileSignaturesConsoleApp.Helpers;
using FileSignaturesConsoleApp.Entities;

namespace FileSignaturesConsoleApp
{
    public class FileSignatureProcessor
    {
        private readonly IHashGenerator _hashGenerator;
        private readonly IFileReader _fileReader;
        private readonly Mutex _segmentsQueueMutex;
        private readonly Mutex _availableWorkerThreadsMutex;

        private CancellationTokenSource CanelationTokenSource { get; set; }

        private Queue<Thread> AvailableWorkerThreads { get; set; }

        private Dictionary<int, WorkerThread> WorkerThreads { get; set; }

        private Queue<Segment> SegmentsQueue { get; set; }

        private bool FileIsRead { get; set; }
        private bool ProcessedSucessfully { get; set; }


        public FileSignatureProcessor(IHashGenerator hashGenerator, IFileReader fileReader)
        {
            _hashGenerator = hashGenerator;
            _fileReader = fileReader;            
            _segmentsQueueMutex = new Mutex(false);
            _availableWorkerThreadsMutex = new Mutex(false);           
        }

        public bool ShowFileSignatures(int workerThreadsNumber)
        {
            this.ProcessedSucessfully = true;
            this.SegmentsQueue = new Queue<Segment>();
            this.FileIsRead = false;
            var finishedWorkEventHandles = StartWorkerThreads(workerThreadsNumber);

            var readerThread = new Thread(() =>
            {
                try
                {
                    _fileReader.Read(ProcessSegment, this.CanelationTokenSource.Token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception during reading the file!\nMessage:\"{ex.Message}\"\nStackTrace: {ex.StackTrace}");
                    this.CanelationTokenSource.Cancel();
                    this.ProcessedSucessfully = false;
                }
            });

            readerThread.Priority = ThreadPriority.AboveNormal;
            readerThread.Start();
            Console.WriteLine($"Thread {readerThread.ManagedThreadId} STARTED reading the file");
            readerThread.Join();
            this.FileIsRead = true;
            Console.WriteLine($"Thread {readerThread.ManagedThreadId} FINISHED reading the file");
            WaitHandle.WaitAll(finishedWorkEventHandles);
            Console.WriteLine($"All worker threads HAVE FINISHED processing data");
            return this.ProcessedSucessfully;
        }

        private EventWaitHandle[] StartWorkerThreads(int workerThreadsNumber)
        {
            this.WorkerThreads = new Dictionary<int, WorkerThread>(workerThreadsNumber);
            this.AvailableWorkerThreads = new Queue<Thread>(workerThreadsNumber);
            this.CanelationTokenSource = new CancellationTokenSource();
            var finishedWorkEventHandles = new EventWaitHandle[workerThreadsNumber];

            for (int i = 0; i < workerThreadsNumber; i++)
            {
                var finishedWorkEventHandle = new ManualResetEvent(false);
                finishedWorkEventHandles[i] = finishedWorkEventHandle;

                var thread = new Thread(() =>
                {
                    try
                    {
                        DisplaySegmentHash();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception during segment processing by work thread {Environment.CurrentManagedThreadId}!\nMessage:\"{ex.Message}\"\nStackTrace: {ex.StackTrace}");
                        this.CanelationTokenSource.Cancel();                       
                        this.ProcessedSucessfully = false;                      
                    }
                    finally
                    {
                        finishedWorkEventHandle.Set();
                    }
                });

                var ewh = new EventWaitHandle(false, EventResetMode.AutoReset);
                var workerThread = new WorkerThread(thread, ewh);
                this.WorkerThreads.Add(thread.ManagedThreadId, workerThread);
                this.AvailableWorkerThreads.Enqueue(thread);
                thread.Start();
            }

            return finishedWorkEventHandles;
        }

        private void ProcessSegment(Segment segment)
        {
            _availableWorkerThreadsMutex.WaitOne();
            var IsWorkerThreadAvailable = this.AvailableWorkerThreads.Any();

            if (IsWorkerThreadAvailable)
            {
                var availableThread = this.AvailableWorkerThreads.Dequeue();
                ScheduleWork(availableThread.ManagedThreadId, segment);
            }

            //can't use if-else, bc of a possible dead lock
            _availableWorkerThreadsMutex.ReleaseMutex();

            if(!IsWorkerThreadAvailable)
            {
                _segmentsQueueMutex.WaitOne();
                this.SegmentsQueue.Enqueue(segment);
                _segmentsQueueMutex.ReleaseMutex();
            }          
        }

        private void DisplaySegmentHash()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"Thread {threadId} STARTED working");

            var workerThread = GetWorkerThread(threadId);
            var cancelationToken = this.CanelationTokenSource.Token;
            var finished = false;

            while (!finished)
            {
                var handleIndex = WaitHandle.WaitAny(new[] { workerThread.EventWaitHandle, cancelationToken.WaitHandle });

                if (cancelationToken.IsCancellationRequested)
                {
                    finished = true;
                }
                else
                {
                    var segment = workerThread.Segment;

                    if(segment == null)
                    {
                        throw new InvalidOperationException("No segment has been set. Operation started before segment has been set");
                    }

                    workerThread.Segment = null;
                    var hash = _hashGenerator.GenerateHash(segment.Data);
                    Console.WriteLine($"Thread: {threadId}, Segment: {segment.Id}, Hash: {Convert.ToHexString(hash)}");
                    ScheduleWorkToCurrentThread();
                }
            }

            Console.WriteLine($"Thread {threadId} FINISHED working");
        }

        private void ScheduleWorkToCurrentThread()
        {
            _segmentsQueueMutex.WaitOne();
            var queueIsEmpty = !this.SegmentsQueue.Any();

            if (!queueIsEmpty)
            {
                var segment = this.SegmentsQueue.Dequeue();
                ScheduleWork(Environment.CurrentManagedThreadId, segment);
            }

            //can't use if else, bc of a possible dead lock
            _segmentsQueueMutex.ReleaseMutex();

            if (queueIsEmpty)
            {
                if (this.FileIsRead)
                {
                    this.CanelationTokenSource.Cancel();
                }
                else
                {
                    _availableWorkerThreadsMutex.WaitOne();
                    this.AvailableWorkerThreads.Enqueue(Thread.CurrentThread);
                    _availableWorkerThreadsMutex.ReleaseMutex();
                }              
            }          
        }

        private void ScheduleWork(int threadId, Segment segment)
        {
            var workerThread = GetWorkerThread(threadId);
            workerThread.Segment = segment;
            workerThread.EventWaitHandle.Set();
        }

        private WorkerThread GetWorkerThread(int threadId)
        {
            if (!this.WorkerThreads.ContainsKey(threadId))
            {
                throw new InvalidOperationException($"Unexpected thread Id: {threadId}, Name: {Thread.CurrentThread.Name}. Only registered worker threads are allowed!");
            }

            var workerThread = this.WorkerThreads[threadId];
            return workerThread;
        }
    }
}
