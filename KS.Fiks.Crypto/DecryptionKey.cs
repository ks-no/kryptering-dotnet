using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;

namespace KS.Fiks.Crypto
{
    /// <summary>
    /// Associates an RSA private key with its certificate for CMS recipient selection.
    /// </summary>
    public sealed class DecryptionKey
    {
        public DecryptionKey(X509Certificate certificate, AsymmetricKeyParameter privateKey)
        {
            Certificate = certificate ?? throw new ArgumentNullException(nameof(certificate));
            PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));

            var rsaPrivateKey = privateKey as RsaKeyParameters;
            if (rsaPrivateKey == null || !rsaPrivateKey.IsPrivate)
            {
                throw new ArgumentException("An RSA private key is required.", nameof(privateKey));
            }

            var publicKey = certificate.GetPublicKey() as RsaKeyParameters;
            if (publicKey == null ||
                !publicKey.Modulus.Equals(rsaPrivateKey.Modulus) ||
                (rsaPrivateKey is RsaPrivateCrtKeyParameters crtKey &&
                 !publicKey.Exponent.Equals(crtKey.PublicExponent)))
            {
                throw new ArgumentException("The private key does not match the certificate.", nameof(privateKey));
            }
        }

        public X509Certificate Certificate { get; }

        public AsymmetricKeyParameter PrivateKey { get; }
    }
}
