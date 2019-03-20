using System;
using System.IO;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.X509;

namespace KS.Fiks.Crypto.BouncyCastle
{
    public static class X509CertificateReader
    {
        public static X509Certificate ExtractCertificate(string pemPublicKey)
        {
            var pemContent = pemPublicKey ?? throw new ArgumentNullException(nameof(pemPublicKey));
            var certificatePem = PemObjectReader.ReadPem(pemContent);
            return ExtractCertificateFromPemObject(certificatePem);
        }

        public static X509Certificate ExtractCertificate(Stream stream)
        {
            return ExtractCertificateFromPemObject(PemObjectReader.ReadPem(stream));
        }

        private static X509Certificate ExtractCertificateFromPemObject(PemObject pemObject)
        {
            if (!pemObject.Type.EndsWith("CERTIFICATE", StringComparison.CurrentCulture))
            {
                throw new ArgumentException("The PEM string is not a valid certificate", nameof(pemObject));
            }

            return new X509Certificate(X509CertificateStructure.GetInstance(pemObject.Content));
        }
    }
}