using System;
using System.IO;
using KS.Fiks.Crypto.BouncyCastle;
using Org.BouncyCastle.Crypto;

namespace KS.Fiks.Crypto
{
    /// <summary>
    /// Service used for decryption of AES encrypted CRM encoded data
    /// </summary>
    public class DecryptionService : IDecryptionService
    {
        private readonly AsymmetricKeyParameter _privateKey;

        internal DecryptionService(AsymmetricKeyParameter privateKey)
        {
            this._privateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
        }

        /// <summary>
        /// Creates a new instance using the provided private key for decryption
        /// </summary>
        /// <param name="privateKey">the private key to be used for decryption</param>
        /// <returns>a new instance of the DecryptionService</returns>
        public static DecryptionService Create(AsymmetricKeyParameter privateKey)
        {
            return new DecryptionService(privateKey);
        }

        /// <summary>
        /// Creates a new instance using the provided DER encoded PEM as private key for decryption
        /// </summary>
        /// <param name="pemStringPrivateKey">a DER encoded PEM containing a private key</param>
        /// <returns>a new instance of the DecryptionService</returns>
        public static DecryptionService Create(string pemStringPrivateKey)
        {
            return Create(AsymmetricKeyParameterReader.ExtractPrivateKey(pemStringPrivateKey));
        }

        public Stream Decrypt(Stream encryptedStream)
        {
            if (!encryptedStream.CanRead)
            {
                throw new ArgumentException("Stream is not readable", nameof(encryptedStream));
            }

            return DecryptStreamConverter.Decrypt(this._privateKey, encryptedStream);
        }
    }
}