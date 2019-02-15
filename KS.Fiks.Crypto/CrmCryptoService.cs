using System;
using System.IO;

namespace KS.Fiks.Crypto
{
    public class CrmCryptoService : ICryptoService
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
            public static CrmCryptoService Create()
            {
                return new CrmCryptoService();
            }
        }
    }
}