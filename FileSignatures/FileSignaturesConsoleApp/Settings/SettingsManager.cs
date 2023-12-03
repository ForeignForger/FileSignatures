namespace FileSignaturesConsoleApp
{
    public class SettingsManager
    {
        private readonly ILogger _logger;
        public SettingsManager(ILogger logger)
        {
            _logger = logger;
        }

        //TODO better to use some library for arguments or at least have arguments names
        public Settings GetSettings(string[] args)
        {
            if (args.Length < 2)
                throw new ArgumentException("Not enough arguments provided. Please set file path and segment size.");

            String filePath = args[0];

            Int32 segmentSize = Int32.Parse(args[1]);
            if (segmentSize <= 0)
                throw new ArgumentException("Incorrect segment size format!. Segment size should be a positive integer.");

            Int32? logBufferSize = null;
            if (args.Length >= 3)
            {
                logBufferSize = Int32.Parse(args[2]);
                if (logBufferSize <= 0)
                    logBufferSize = null;
            }

            QueueSettings queueSettings = new QueueSettings();
            if (args.Length >= 5)
            {
                Int32 queueCooldownTime = Int32.Parse(args[3]);
                Int32 maxQueueSize = Int32.Parse(args[4]);

                if (queueCooldownTime <= 0 && maxQueueSize > 0)
                    _logger.LogWarning("MaxQueueSize parameter doesn't affect the application if Cooldowntime parameter is zero/turned down");

                if (queueCooldownTime > 0 && maxQueueSize <= 0)
                    _logger.LogWarning("Cooldowntime parameter doesn't affect the application if MaxQueueSize parameter is zero/turned down");

                queueSettings = new QueueSettings(maxQueueSize, queueCooldownTime);
            }

            Boolean doCountDown = false;
            if(args.Length >= 6)
                doCountDown = Boolean.Parse(args[5]);

            Int32 workThreadsCount = Environment.ProcessorCount;
            Settings settings = new Settings(filePath, segmentSize, workThreadsCount, queueSettings, logBufferSize, doCountDown);
            return settings;
        }

    }
}
