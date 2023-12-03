namespace FileSignaturesConsoleApp
{
    public interface IFileReader
    {
        IEnumerable<Segment> Read(CancellationToken token);
    }
}
