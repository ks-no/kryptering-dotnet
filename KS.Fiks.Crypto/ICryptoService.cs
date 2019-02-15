using System.IO;

namespace KS.Fiks.Crypto
{
    public interface ICryptoService
    {
        Stream Decrypt(Stream encryptedStream);

        Stream Encrypt(Stream unEncryptedStream);
    }
}