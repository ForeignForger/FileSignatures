using FileSignaturesConsoleApp.FileReaders;
using FileSignaturesConsoleApp.Helpers;

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

            var filePath = args[0];
            int segmentSize;
            var parced  = Int32.TryParse(args[1], out segmentSize);

            if(!parced || segmentSize <= 0)
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
                var fileSignatureProcessor = new FileSignatureProcessor(hashGenerator, fileReader);
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