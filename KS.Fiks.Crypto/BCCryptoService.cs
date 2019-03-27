using System;
using System.IO;
using KS.Fiks.Crypto.BouncyCastle;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Operators;
using Org.BouncyCastle.X509;

namespace KS.Fiks.Crypto
{
    /// <summary>
    /// Implementation of ICryptoService based on BouncyCastle C# portable
    /// </summary>
    public class BCCryptoService : ICryptoService
    {
        private const string AsnAlgoritm = "RSA/NONE/OAEPWITHSHA256ANDMGF1PADDING";
        private readonly X509Certificate certificate;
        private readonly AsymmetricKeyParameter privateKey;

        private BCCryptoService(X509Certificate certificate, AsymmetricKeyParameter privateKey)
        {
            this.certificate = certificate;
            this.privateKey = privateKey;
        }

        /// <summary>
        /// Creates an instance of the BCCryptoService
        /// </summary>
        /// <param name="certificate">the public key to be used for encryption</param>
        /// <param name="privateKey">the private key to be used for decryption</param>
        /// <returns>A new instance of BCCryptoService</returns>
        /// <exception cref="ArgumentNullException">If either parameter is null</exception>
        public static BCCryptoService Create(X509Certificate certificate, AsymmetricKeyParameter privateKey)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            if (privateKey == null)
            {
                throw new ArgumentNullException(nameof(privateKey));
            }

            return new BCCryptoService(certificate, privateKey);
        }

        /// <summary>
        /// Creates an instance 
        /// </summary>
        /// <param name="pemPublicKeyString"></param>
        /// <param name="pemPrivateKeyString"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static BCCryptoService Create(string pemPublicKeyString, string pemPrivateKeyString)
        {
            if (pemPublicKeyString == null)
            {
                throw new ArgumentNullException(nameof(pemPublicKeyString));
            }

            if (pemPrivateKeyString == null)
            {
                throw new ArgumentNullException(nameof(pemPrivateKeyString));
            }

            return Create(
                X509CertificateReader.ExtractCertificate(pemPublicKeyString),
                AsymmetricKeyParameterReader.ExtractPrivateKey(pemPrivateKeyString));
        }

        public Stream Decrypt(Stream encryptedStream)
        {
            return DecryptStreamConverter.Decrypt(this.privateKey, encryptedStream);
        }

        public void Encrypt(Stream unEncryptedStream, Stream encryptedOutStream)
        {
            var cmsEnvelopedDataStreamGenerator = new CmsEnvelopedDataStreamGenerator();
            cmsEnvelopedDataStreamGenerator.AddKeyTransRecipient(this.certificate);
            cmsEnvelopedDataStreamGenerator.AddRecipientInfoGenerator(new CmsKeyTransRecipientInfoGenerator(
                this.certificate, new Asn1KeyWrapper(AsnAlgoritm, this.certificate)));
            using (var encryptedStream =
                cmsEnvelopedDataStreamGenerator.Open(encryptedOutStream, CmsEnvelopedGenerator.Aes256Cbc))
            {
                unEncryptedStream.CopyTo(encryptedStream);
            }
        }
    }
}