using FileSignaturesConsoleApp.Entities;

namespace FileSignaturesConsoleApp.FileReaders
{
    public interface IFileReader
    {
        void Read(Action<Segment> sendSegment, CancellationToken token);
    }
}
