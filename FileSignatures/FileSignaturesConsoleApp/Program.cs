using System.Text.Json;

namespace FileSignaturesConsoleApp
{
    public class Program
    {
        private const int COUNTDOWN_SEC = 10;

        private static readonly ILogger _logger;
        private static readonly SettingsManager _settingsManager;

        static Program()
        {
            
            _logger = new ConsoleLogger();
            _settingsManager = new SettingsManager(_logger);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">{file path}:REQUIRED {segment size}:REQUIRED {log buffer size} {cool down time} {max queue size}</param>
        public static void Main(string[] args)
        {
            //TODO figure out how to know what queueSize and cool down time is the best to choose.
            //I guess we need to check the current system to know this.
            //How much memory is available for us and how fast the data is being proccessed.
            //Queue settings are important to have free memory available for processing and logging purposes, otherwise the program is basically locks itself after eating all available memory
            try
            {
                Settings settings = _settingsManager.GetSettings(args);
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
        private static void Countdown(Settings settings)
        {
            if (!settings.DoCountDown)
                return;

            _logger.LogInfo($"Program will start in {COUNTDOWN_SEC} seconds...");
            for (int i = 0; i < COUNTDOWN_SEC; i++)
            {
                _logger.LogInfo($"{COUNTDOWN_SEC - i}...");
                Thread.Sleep(1000);
            }
        }

        private static FileSignatureProcessor InitFileSignatureProcessor(Settings settings)
        {
            IFileReader fileReader = new FileStreamReader(settings.FilePath, settings.SegmentSize);
            IHashGenerator hashGenerator = new SHA256HashGenerator();
            IBufferedLogger logger = new BufferedConsoleLoger(settings.LogBufferSize);
            FileSignatureProcessor fileSignatureProcessor = new FileSignatureProcessor(hashGenerator, fileReader, logger, settings.QueueSettings);
            return fileSignatureProcessor;
        }
    }
}