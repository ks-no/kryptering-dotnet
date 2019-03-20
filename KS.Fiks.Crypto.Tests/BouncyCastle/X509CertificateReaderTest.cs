using FluentAssertions;
using KS.Fiks.Crypto.BouncyCastle;
using Org.BouncyCastle.Math;
using Xunit;

namespace KS.Fiks.Crypto.Tests.BouncyCastle
{
    public class X509CertificateReaderTest
    {
        private const string IssuerDN = "C=NO,ST=kryptering,L=kryptering,O=kryptering,OU=kryptering,CN=kryptering";
        private const string SubjectDN = "C=NO,ST=kryptering,L=kryptering,O=kryptering,OU=kryptering,CN=kryptering";

        [Fact(DisplayName = "Extract certificate from Stream")]
        public void ReadCertificateFromStream()
        {
            using (var pemStream = TestDataUtil.GetContentStreamFromResource("fiks_demo_public.pem"))
            {
                var certificate = X509CertificateReader.ExtractCertificate(pemStream);
                certificate.Should().NotBeNull();
                certificate.IssuerDN.ToString().Should().Be(IssuerDN);
                certificate.SubjectDN.ToString().Should().Be(SubjectDN);
            }
        }

        [Fact(DisplayName = "Extract certificate from string")]
        public void ReadValidCertificate()
        {
            var pemFile = TestDataUtil.GetContentFromResource("fiks_demo_public.pem");
            var certificate = X509CertificateReader.ExtractCertificate(pemFile);
            certificate.Should().NotBeNull();
            certificate.IssuerDN.ToString().Should().Be(IssuerDN);
            certificate.SubjectDN.ToString().Should().Be(SubjectDN);
            certificate.SigAlgName.Should().Be("SHA-256withRSA");
            certificate.SerialNumber.Should().Be(new BigInteger("9386322399984342181"));
        }
    }
}