using System;
using System.IO;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.X509;

namespace KS.Fiks.Crypto
{
    public class BCCryptoService : ICryptoService
    {
        private readonly X509Certificate certificate;

        public BCCryptoService(X509Certificate certificate)
        {
            this.certificate = certificate;
        }

        public Stream Decrypt(Stream encryptedStream)
        {
            throw new NotImplementedException();
        }

        public Stream Encrypt(Stream unEncryptedStream)
        {
            var encryptionStream = CreateEncryptionStream();
            unEncryptedStream.CopyTo(encryptionStream);
            encryptionStream.Seek(0L, SeekOrigin.Begin);
            return encryptionStream;
        }

        public Stream CreateEncryptionStream()
        {
            var cmsEnvelopedDataStreamGenerator = new CmsEnvelopedDataStreamGenerator();
            cmsEnvelopedDataStreamGenerator.AddKeyTransRecipient(this.certificate);
            return new EncodedStream(new MemoryStream(), cmsEnvelopedDataStreamGenerator);
        }

        public static class Factory
        {
            public static BCCryptoService Create(X509Certificate certificate)
            {
                if (certificate == null) throw new ArgumentNullException(nameof(certificate));

                return new BCCryptoService(certificate);
            }
        }
    }
}