using System.IO;

namespace KS.Fiks.Crypto
{
    public class OpenSslCryptoService : ICryptoService
    {
        public static class Factory
        {
            public static OpenSslCryptoService Create()
            {
                return new OpenSslCryptoService();
            }
        }

        public Stream Decrypt(Stream encryptedStream)
        {
            throw new System.NotImplementedException();
        }

        public Stream Encrypt(Stream unEncryptedStream)
        {
            throw new System.NotImplementedException();
        }
    }
}