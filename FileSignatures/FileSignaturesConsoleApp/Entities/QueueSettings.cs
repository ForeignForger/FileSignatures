namespace FileSignaturesConsoleApp.Entities
{
    /// <summary>
    /// Settings for FileSignatureProcessor queue
    /// </summary>
    /// <param name="MaxQueueSize">Max size of the queue</param>
    /// <param name="CooldownTime">Thread sleeping time in milliseconeds if it reached maximum queue size</param>
    public record class QueueSettings(int MaxQueueSize = -1, int CooldownTime = -1);
}
