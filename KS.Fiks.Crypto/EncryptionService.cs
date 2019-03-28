using System;
using System.IO;
using KS.Fiks.Crypto.BouncyCastle;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Operators;
using Org.BouncyCastle.X509;

namespace KS.Fiks.Crypto
{
    public class EncryptionService : IEncryptionService
    {
        private const string AsnAlgoritm = "RSA/NONE/OAEPWITHSHA256ANDMGF1PADDING";
        private readonly X509Certificate _certificate;

        internal EncryptionService(X509Certificate certificate)
        {
            this._certificate = certificate ?? throw new ArgumentNullException(nameof(certificate));
        }

        public static EncryptionService Create(X509Certificate certificate)
        {
            return new EncryptionService(certificate);
        }

        public static EncryptionService Create(string pemPublicCertString)
        {
            return Create(X509CertificateReader.ExtractCertificate(pemPublicCertString));
        }

        public void Encrypt(Stream unEncryptedStream, Stream encryptedOutStream)
        {
            var cmsEnvelopedDataStreamGenerator = new CmsEnvelopedDataStreamGenerator();
            cmsEnvelopedDataStreamGenerator.AddKeyTransRecipient(this._certificate);
            cmsEnvelopedDataStreamGenerator.AddRecipientInfoGenerator(new CmsKeyTransRecipientInfoGenerator(
                this._certificate, new Asn1KeyWrapper(AsnAlgoritm, this._certificate)));
            using (var encryptedStream =
                cmsEnvelopedDataStreamGenerator.Open(encryptedOutStream, CmsEnvelopedGenerator.Aes256Cbc))
            {
                unEncryptedStream.CopyTo(encryptedStream);
            }
        }
    }
}