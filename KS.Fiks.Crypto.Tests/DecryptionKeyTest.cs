using System;
using KS.Fiks.Crypto.BouncyCastle;
using Shouldly;
using Xunit;

namespace KS.Fiks.Crypto.Tests
{
    public class DecryptionKeyTest
    {
        [Fact]
        public void AcceptsMatchingCertificateAndPrivateKey()
        {
            var certificate = TestDataUtil.ReadPublicCertificate();
            var privateKey = TestDataUtil.ReadPrivateKey();

            var key = new DecryptionKey(certificate, privateKey);

            key.Certificate.ShouldBeSameAs(certificate);
            key.PrivateKey.ShouldBeSameAs(privateKey);
        }

        [Fact]
        public void RejectsNullCertificateOrKey()
        {
            Should.Throw<ArgumentNullException>(() => new DecryptionKey(null, TestDataUtil.ReadPrivateKey()));
            Should.Throw<ArgumentNullException>(() => new DecryptionKey(TestDataUtil.ReadPublicCertificate(), null));
        }

        [Fact]
        public void RejectsPublicKey()
        {
            var certificate = TestDataUtil.ReadPublicCertificate();

            Should.Throw<ArgumentException>(() => new DecryptionKey(certificate, certificate.GetPublicKey()));
        }

        [Fact]
        public void RejectsMismatchedPrivateKey()
        {
            var privateKey = AsymmetricKeyParameterReader.ExtractPrivateKey(
                TestDataUtil.GetContentFromResource("rsa_private_key.pem"));

            Should.Throw<ArgumentException>(() => new DecryptionKey(TestDataUtil.ReadPublicCertificate(), privateKey));
        }
    }
}
