using FileSignaturesConsoleApp.Entities;

namespace FileSignaturesConsoleApp.FileReaders
{
    public interface IFileReader
    {
        IEnumerable<Segment> Read(CancellationToken token);
    }
}
