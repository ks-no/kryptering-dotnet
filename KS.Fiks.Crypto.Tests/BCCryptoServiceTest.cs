using System.IO;
using System.Text;
using FluentAssertions;
using Org.BouncyCastle.Utilities.Encoders;
using Xunit;

namespace KS.Fiks.Crypto.Tests
{
    public class BCCryptoServiceTest
    {
        [Fact(DisplayName = "Create Bouncy Castle Crypto Service")]
        public void Create()
        {
            var cryptoService = CreateCryptoServiceForTest();
            cryptoService.Should().BeOfType<BCCryptoService>();
        }

        [Fact(DisplayName = "Perform decryption")]
        public void Decrypt()
        {
            var unencryptedDataChunk = TestDataUtil.GetContentFromResource("UnencryptedData.txt");
            var encryptedData = Base64.Decode(TestDataUtil.GetContentFromResource("EncryptedData.txt"));
            var cryptoService = CreateCryptoServiceForTest();
            using (var encryptedDataStream = new MemoryStream(encryptedData))
            {
                using (var decryptedDataStream = cryptoService.Decrypt(encryptedDataStream))
                using (var decryptedStuff = new MemoryStream())
                {
                    decryptedDataStream.CopyTo(decryptedStuff);
                    var decryptedDataChunk = decryptedStuff.ToArray();
                    var decryptedDataString = Encoding.UTF8.GetString(decryptedDataChunk);

                    decryptedDataString.Should().Be(unencryptedDataChunk);
                }
            }
        }

        [Fact(DisplayName = "Perform encryption")]
        public void Encrypt()
        {
            var testDataContent = GetTestDataFromResource();
            var testData = Encoding.UTF8.GetBytes(testDataContent);

            var cryptoService = CreateCryptoServiceForTest();
            byte[] encryptedData = null;
            using (var encryptedStream = new MemoryStream())
            using (var unencryptedStream = new MemoryStream(testData))
            {
                cryptoService.Encrypt(unencryptedStream, encryptedStream);
                encryptedData = encryptedStream.ToArray();
            }

            using (var encryptedStream = new MemoryStream(encryptedData))
            using (var decryptedStream = cryptoService.Decrypt(encryptedStream))
            using (var decryptionBuffer = new MemoryStream())
            {
                decryptedStream.CopyTo(decryptionBuffer);
                var decryptedData = decryptionBuffer.ToArray();
                var decryptedString = Encoding.UTF8.GetString(decryptedData);
                decryptedString.Should().Be(testDataContent);
            }
        }

        private static BCCryptoService CreateCryptoServiceForTest()
        {
            return BCCryptoService.Create(ReadCertificatePem(), ReadPrivateKeyPem());
        }

        private static string ReadCertificatePem()
        {
            return TestDataUtil.GetContentFromResource("fiks_demo_public.pem");
        }

        private static string ReadPrivateKeyPem()
        {
            return TestDataUtil.GetContentFromResource("fiks_demo_private.pem");
        }

        private static string GetTestDataFromResource()
        {
            return TestDataUtil.GetContentFromResource("LoremIpsum.txt");
        }
    }
}