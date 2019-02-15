using System.IO;

namespace KS.Fiks.Crypto
{
    public class CrmCryptoService : ICryptoService
    {
        public static class Factory
        {
            public static CrmCryptoService Create()
            {
                return new CrmCryptoService();
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