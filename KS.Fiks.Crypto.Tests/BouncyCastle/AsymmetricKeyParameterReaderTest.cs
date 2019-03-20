using FluentAssertions;
using KS.Fiks.Crypto.BouncyCastle;
using Xunit;

namespace KS.Fiks.Crypto.Tests.BouncyCastle
{
    public class AsymmetricKeyParameterReaderTest
    {
        [Fact(DisplayName = "Extract private key")]
        public void ExtractPrivateKey()
        {
            var privateKeyString = TestDataUtil.GetContentFromResource("fiks_demo_private.pem");
            var asymmetricKeyParameter = AsymmetricKeyParameterReader.ExtractPrivateKey(privateKeyString);
            asymmetricKeyParameter.Should().NotBeNull();
            asymmetricKeyParameter.IsPrivate.Should().BeTrue();
        }

        [Fact(DisplayName = "Extract private RSA key")]
        public void ExtractPrivateRSAKey()
        {
            var privateRsaKeyString = TestDataUtil.GetContentFromResource("rsa_private_key.pem");
            var asymmetricKeyParameter = AsymmetricKeyParameterReader.ExtractPrivateKey(privateRsaKeyString);
            asymmetricKeyParameter.Should().NotBeNull();
            asymmetricKeyParameter.IsPrivate.Should().BeTrue();
        }
    }
}