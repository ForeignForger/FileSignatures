using FileSignaturesConsoleApp.FileReaders;
using FileSignaturesConsoleApp.Helpers;
using FileSignaturesConsoleApp.Loggers;

namespace FileSignaturesConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Not enough arguments provided.\n Please set file path and segment size.");
                return;
            }

            //TODO figure out how to know what queueSize and cool down time is the best too choose.
            //I guess we need to check the current system to know this.
            //How much memory is available for us and how fast the data is being proccessed
            //queue settings are important to have free memory available for processing and logging purposes, otherwise the progrma is basically locks itself
            var filePath = args[0];
            int segmentSize;
            var parced  = Int32.TryParse(args[1], out segmentSize);
            int logBufferSize = 1000;
            int maxQueueSize = 500000;
            int queueCoolDownMilliseconds = 500;

            if (!parced || segmentSize <= 0)
            {
                Console.WriteLine("Incorrect segment size format!. Segment size should be a positive integer.");
                return;
            }

            var threadCount = Environment.ProcessorCount;
            Console.WriteLine($"File segments signatures. File: {filePath}, Segment size: {segmentSize}");

            try
            {
                IFileReader fileReader = new FileStreamReader(filePath, segmentSize);
                IHashGenerator hashGenerator = new SHA256HashGenerator();
                IBufferedLogger logger = new BufferedConsoleLoger(logBufferSize);
                var fileSignatureProcessor = new FileSignatureProcessor(hashGenerator, fileReader, logger, maxQueueSize, queueCoolDownMilliseconds);
                var success = fileSignatureProcessor.ShowFileSignatures(threadCount);
                var status = success ? "SUCCESSFULLY" : "UNSUCCESSFULLY";
                Console.WriteLine($"File processing has been finished {status}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected exception: \"{ex.Message}\"\nStackTrace: {ex.StackTrace}");
            }

        }
    }
}