using System;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace KS.Fiks.Crypto.BouncyCastle
{
    public static class AsymmetricKeyParameterReader
    {
        private const string RsaSuffix = "RSA PRIVATE KEY";

        public static AsymmetricKeyParameter ExtractPrivateKey(string pemPrivateKey)
        {
            var pemObject = PemObjectReader.ReadPem(pemPrivateKey);
            if (!pemObject.Type.EndsWith(RsaSuffix, StringComparison.CurrentCulture))
            {
                return PrivateKeyFactory.CreateKey(pemObject.Content);
            }

            var rsaPrivateKeyStructure = RsaPrivateKeyStructure.GetInstance(pemObject.Content);
            return new RsaPrivateCrtKeyParameters(
                rsaPrivateKeyStructure.Modulus,
                rsaPrivateKeyStructure.PublicExponent,
                rsaPrivateKeyStructure.PrivateExponent,
                rsaPrivateKeyStructure.Prime1,
                rsaPrivateKeyStructure.Prime2,
                rsaPrivateKeyStructure.Exponent1,
                rsaPrivateKeyStructure.Exponent2,
                rsaPrivateKeyStructure.Coefficient);
        }
    }
}