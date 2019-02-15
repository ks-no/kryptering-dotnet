using System;
using System.IO;

namespace KS.Fiks.Crypto
{
    public class OpenSslCryptoService : ICryptoService
    {
        public Stream Decrypt(Stream encryptedStream)
        {
            throw new NotImplementedException();
        }

        public Stream Encrypt(Stream unEncryptedStream)
        {
            throw new NotImplementedException();
        }

        public static class Factory
        {
            public static OpenSslCryptoService Create()
            {
                return new OpenSslCryptoService();
            }
        }
    }
}