using FileSignaturesConsoleApp.Entities;
using FileSignaturesConsoleApp.FileReaders;
using FileSignaturesConsoleApp.Helpers;
using FileSignaturesConsoleApp.Loggers;
using System.Text.Json;

namespace FileSignaturesConsoleApp
{
    public class Program
    {
        private const int _defaultLogBufferSize = 1000;
        private const bool _doCountDown = false;
        private const int _countdownSeconds = 10;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">{file path} {segment size} {log buffer size} {cool down time} {max queue size}</param>
        public static void Main(string[] args)
        {
            //TODO figure out how to know what queueSize and cool down time is the best to choose.
            //I guess we need to check the current system to know this.
            //How much memory is available for us and how fast the data is being proccessed.
            //Queue settings are important to have free memory available for processing and logging purposes, otherwise the program is basically locks itself after eating all available memory
            try
            {
                var settings = GetSettings(args);
                Console.WriteLine($"File segments signatures. Settings: {JsonSerializer.Serialize(settings)}");

                if (_doCountDown)
                {
                    Console.WriteLine($"Program will start in {_countdownSeconds} seconds...");
                    Countdown();
                }

                IFileReader fileReader = new FileStreamReader(settings.FilePath, settings.SegmentSize);
                IHashGenerator hashGenerator = new SHA256HashGenerator();
                IBufferedLogger logger = new BufferedConsoleLoger(settings.LogBufferSize);
                var fileSignatureProcessor = new FileSignatureProcessor(hashGenerator, fileReader, logger, settings.QueueSettings);
                var success = fileSignatureProcessor.ShowFileSignatures(settings.WorkThreadsCount);
                var status = success ? "SUCCESSFULLY" : "UNSUCCESSFULLY";
                Console.WriteLine($"File processing has been finished {status}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected exception: \"{ex.Message}\"\nStackTrace: {ex.StackTrace}");
            }
        }

        private static void Countdown()
        {
            for(int i = 0; i < _countdownSeconds; i++)
            {
                Console.WriteLine($"{_countdownSeconds - i}...");
                Thread.Sleep(1000);
            }
        }

        //TODO better to use some library for arguments or at least have arguments names
        private static Settings GetSettings(string[] args)
        {
            var settings = new Settings();

            if (args.Length < 2)
            {
                throw new ArgumentException("Not enough arguments provided. Please set file path and segment size.");
            }

            settings.FilePath = args[0];
            int segmentSize;

            if (Int32.TryParse(args[1], out segmentSize) && segmentSize > 0)
            {
                settings.SegmentSize = segmentSize;
            }
            else
            {
                throw new ArgumentException("Incorrect segment size format!. Segment size should be a positive integer.");
            }

            int logBufferSize = -1;

            if (args.Length >= 3)
            {
                logBufferSize = int.Parse(args[2]);
            }

            settings.LogBufferSize = logBufferSize <= 0 ? _defaultLogBufferSize : logBufferSize;
           
            if(args.Length >= 5)
            {
                int queueCooldownTime = Int32.Parse(args[3]);
                int maxQueueSize = Int32.Parse(args[4]);

                if(queueCooldownTime <= 0 && maxQueueSize > 0)
                {
                    Console.WriteLine("WARNING: MaxQueueSize parameter doesn't affect the application if Cooldowntime parameter is zero/turned down");
                }

                if (queueCooldownTime > 0 && maxQueueSize <= 0)
                {
                    Console.WriteLine("WARNING: Cooldowntime parameter doesn't affect the application if MaxQueueSize parameter is zero/turned down");
                }

                settings.QueueSettings = new QueueSettings(maxQueueSize, queueCooldownTime);
            }
            else
            {
                settings.QueueSettings = new QueueSettings();
            }

            settings.WorkThreadsCount = Environment.ProcessorCount;

            return settings;
        }
    }
}