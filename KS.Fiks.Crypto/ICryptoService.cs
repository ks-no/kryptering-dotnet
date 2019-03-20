using System.IO;

namespace KS.Fiks.Crypto
{
    public interface ICryptoService
    {
        Stream Decrypt(Stream encryptedStream);

        void Encrypt(Stream unEncryptedStream, Stream encryptedOutStream);
    }
}