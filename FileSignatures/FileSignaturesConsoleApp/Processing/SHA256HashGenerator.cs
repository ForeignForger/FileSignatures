using System.Security.Cryptography;

namespace FileSignaturesConsoleApp
{
    //Note: tried using IDisposable but ComputeHash() throws error after a while. 
    public class SHA256HashGenerator : IHashGenerator
    {
        public SHA256HashGenerator()
        {
        }

        public byte[] GenerateHash(byte[] data)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(data);
                return hash;
            }
        }
    }
}
