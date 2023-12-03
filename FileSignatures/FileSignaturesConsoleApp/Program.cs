using System.Text.Json;

namespace FileSignaturesConsoleApp
{
    public class Program
    {
        private const bool _doCountDown = false;
        private const int _countdownSeconds = 10;
        private static readonly ILogger _logger;

        static Program()
        {
            _logger = new ConsoleLogger();
        }

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
                Settings settings = GetSettings(args);
                _logger.LogInfo($"File segments signatures. Settings: {JsonSerializer.Serialize(settings)}");

                Countdown(settings);

                FileSignatureProcessor fileSignatureProcessor = InitFileSignatureProcessor(settings);
                Boolean success = fileSignatureProcessor.ShowFileSignatures(settings.WorkThreadsCount);
                String status = success ? "SUCCESSFULLY" : "UNSUCCESSFULLY";
                _logger.LogInfo($"File processing has been finished {status}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected exception: \"{ex.Message}\"\nStackTrace: {ex.StackTrace}");
            }
        }


        //TODO better to use some library for arguments or at least have arguments names
        private static Settings GetSettings(string[] args)
        {
            Settings settings = new Settings();

            if (args.Length < 2)
            {
                throw new ArgumentException("Not enough arguments provided. Please set file path and segment size.");
            }

            settings.FilePath = args[0];
            Int32 segmentSize;

            if (Int32.TryParse(args[1], out segmentSize) && segmentSize > 0)
            {
                settings.SegmentSize = segmentSize;
            }
            else
            {
                throw new ArgumentException("Incorrect segment size format!. Segment size should be a positive integer.");
            }

            if (args.Length >= 3)
            {
                settings.LogBufferSize = Int32.Parse(args[2]);
            }

            if (args.Length >= 5)
            {
                Int32 queueCooldownTime = Int32.Parse(args[3]);
                Int32 maxQueueSize = Int32.Parse(args[4]);

                if (queueCooldownTime <= 0 && maxQueueSize > 0)
                {
                    _logger.LogWarning("MaxQueueSize parameter doesn't affect the application if Cooldowntime parameter is zero/turned down");
                }

                if (queueCooldownTime > 0 && maxQueueSize <= 0)
                {
                    _logger.LogWarning("Cooldowntime parameter doesn't affect the application if MaxQueueSize parameter is zero/turned down");
                }

                settings.QueueSettings = new QueueSettings(maxQueueSize, queueCooldownTime);
            }

            settings.WorkThreadsCount = Environment.ProcessorCount;

            return settings;
        }

        private static FileSignatureProcessor InitFileSignatureProcessor(Settings settings)
        {
            IFileReader fileReader = new FileStreamReader(settings.FilePath, settings.SegmentSize);
            IHashGenerator hashGenerator = new SHA256HashGenerator();
            IBufferedLogger logger = new BufferedConsoleLoger(settings.LogBufferSize);
            FileSignatureProcessor fileSignatureProcessor = new FileSignatureProcessor(hashGenerator, fileReader, logger, settings.QueueSettings);
            return fileSignatureProcessor;
        }

        private static void Countdown(Settings settings)
        {
            if (!settings.DoCountDown)
                return;

            _logger.LogInfo($"Program will start in {_countdownSeconds} seconds...");
            for (int i = 0; i < _countdownSeconds; i++)
            {
                _logger.LogInfo($"{_countdownSeconds - i}...");
                Thread.Sleep(1000);
            }
        }
    }
}