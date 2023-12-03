namespace FileSignaturesConsoleApp
{
    public interface IHashGenerator
    {
        byte[] GenerateHash(byte[] data);
    }
}
