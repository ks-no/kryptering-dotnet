using System.IO;
using System.Text;
using FluentAssertions;
using Org.BouncyCastle.Utilities.Encoders;
using Xunit;

namespace KS.Fiks.Crypto.Tests
{
    public class BCCryptoServiceTest
    {
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

        [Fact(DisplayName = "Create Bouncy Castle Crypto Service")]
        public void Create()
        {
            var cryptoService = CreateCryptoServiceForTest();
            cryptoService.Should().BeOfType<BCCryptoService>();
        }

        [Fact(DisplayName = "Create EncryptStream")]
        public void CreateEncryptStream()
        {
            using (var encryptionStream = CreateCryptoServiceForTest().CreateEncryptionStream())
            {
                encryptionStream.Should().NotBeNull();
                var testData = GetTestDataFromResource();
                using (var testDataStream = new MemoryStream(Encoding.UTF8.GetBytes(testData)))
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

        [Fact(DisplayName = "Perform encryption", Skip = "true")]
        public void Encrypt()
        {
            var testDataContent = GetTestDataFromResource();
            var testData = Encoding.UTF8.GetBytes(testDataContent);

            var cryptoService = CreateCryptoServiceForTest();
            using (var unencryptedStream = new MemoryStream(testData))
            using (var cryptoStream = cryptoService.Encrypt(unencryptedStream))
            {
                using (var encryptedOutStream = new MemoryStream())
                {
                    cryptoStream.CopyTo(encryptedOutStream);
                    var base64EncryptedData = Base64.ToBase64String(encryptedOutStream.ToArray());
                    var preEncryptedBase64Data = TestDataUtil.GetContentFromResource("EncryptedData.txt");
                    preEncryptedBase64Data.Should().Be(base64EncryptedData);
                }
            }
        }
    }
}