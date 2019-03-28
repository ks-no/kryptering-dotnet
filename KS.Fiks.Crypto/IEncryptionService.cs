using System.IO;

namespace KS.Fiks.Crypto
{
    public interface IEncryptionService
    {
        void Encrypt(Stream unEncryptedStream, Stream encryptedOutStream);
    }
}