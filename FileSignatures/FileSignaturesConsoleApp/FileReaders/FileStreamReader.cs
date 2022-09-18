using FileSignaturesConsoleApp.Entities;

namespace FileSignaturesConsoleApp.FileReaders
{
    public class FileStreamReader : IFileReader
    {
        private readonly string _path;
        private readonly int _bufferSize;

        public FileStreamReader(string path, int bufferSize)
        {
            _path = path;
            _bufferSize = bufferSize;
        }

        public IEnumerable<Segment> Read(CancellationToken token)
        {
            if (!File.Exists(_path))
            {
                throw new FileNotFoundException($"Provided file {_path} was not found");
            }
            else
            {
                using(var fsReader = File.OpenRead(_path))
                {
                    var buffer = new byte[_bufferSize];
                    
                    for(int bufferIndex = 0, curSize; (curSize = fsReader.Read(buffer, 0, _bufferSize)) > 0; bufferIndex++)
                    {
                        token.ThrowIfCancellationRequested();
                        var bufferCopy = new byte[curSize];
                        Array.Copy(buffer, bufferCopy, curSize);
                        var segment = new Segment(bufferCopy, bufferIndex);
                        yield return segment;
                    }
                }
            }
        }
    }
}
