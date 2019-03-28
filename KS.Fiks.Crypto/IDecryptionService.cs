using System.IO;

namespace KS.Fiks.Crypto
{
    public interface IDecryptionService
    {
        Stream Decrypt(Stream encryptedStream);
    }
}