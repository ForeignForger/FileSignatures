namespace FileSignaturesConsoleApp.Helpers
{
    public interface IHashGenerator
    {
        byte[] GenerateHash(byte[] data);
    }
}
