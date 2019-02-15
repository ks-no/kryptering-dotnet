using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Org.BouncyCastle.X509;
using Xunit;

namespace KS.Fiks.Crypto.Tests
{
    public class BCCryptoServiceTest
    {
        [Fact(DisplayName = "Create Bouncy Castle Crypto Service")]
        public void Create()
        {
            var bcCryptoService = CreateCryptoServiceForTest();
            bcCryptoService.Should().BeOfType<BCCryptoService>();
        }

        [Fact(DisplayName = "Create EncryptStream")]
        public void CreateEncryptStream()
        {
            using (var encryptionStream = CreateCryptoServiceForTest().CreateEncryptionStream())
            {
                encryptionStream.Should().NotBeNull();
                var testData = GenerateTestData(2000);
                using (var testDataStream = new MemoryStream(testData))
                {
                    testDataStream.CopyTo(encryptionStream);
                }

                encryptionStream.Length.Should().BeGreaterThan(0L);
                using (var encryptedBufferStream = new MemoryStream())
                {
                    encryptionStream.Seek(0L, SeekOrigin.Begin);
                    encryptionStream.CopyTo(encryptedBufferStream);
                    encryptedBufferStream.Length.Should().BeGreaterThan(0);
                }
            }
        }

        [Fact(DisplayName = "Perform encryption")]
        public void Encrypt()
        {
            var testData = GenerateTestData(2000);

            using (var memoryStream = new MemoryStream(testData))
            using (var encryptedStream = CreateCryptoServiceForTest().Encrypt(memoryStream))
            {
                encryptedStream.Should().NotBeNull();
                encryptedStream.CanRead.Should().BeTrue();
                using (var outStream = new MemoryStream(2000))
                {
                    encryptedStream.CopyTo(outStream);
                    outStream.Length.Should().BeGreaterThan(0);
                    outStream.ToArray().Should().NotBeEmpty();
                }
            }
        }

        private static BCCryptoService CreateCryptoServiceForTest()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("fiks_demo_public.pem"));

            X509Certificate x509Certificate;
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                x509Certificate = new X509CertificateParser().ReadCertificate(stream);
            }

            return BCCryptoService.Factory.Create(x509Certificate);
        }

        private static byte[] GenerateTestData(int size)
        {
            var data = new byte[size];
            for (var i = 0; i < size; i++)
            {
                data[i] = (byte) (i & 0xff);
            }

            return data;
        }
    }
}