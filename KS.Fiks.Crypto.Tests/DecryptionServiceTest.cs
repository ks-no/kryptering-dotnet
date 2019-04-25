using System.IO;
using System.Text;
using FluentAssertions;
using Org.BouncyCastle.Utilities.Encoders;
using Xunit;

namespace KS.Fiks.Crypto.Tests
{
    public class DecryptionServiceTest
    {
        [Fact(DisplayName = "Create using PEM")]
        public void CreatePem()
        {
            DecryptionService.Create(TestDataUtil.ReadPrivateKeyPem()).Should().NotBeNull();
        }

        [Fact(DisplayName = "Create using private key")]
        public void CreatePrivateKey()
        {
            DecryptionService.Create(TestDataUtil.ReadPrivateKey()).Should().NotBeNull();
        }

        [Fact(DisplayName = "Decryption")]
        public void Decrypt()
        {
            var unencryptedDataChunk = getUnEncryptedDataFromResource();
            var encryptedData = Base64.Decode(TestDataUtil.GetContentFromResource("EncryptedData.txt"));
            var cryptoService = CreateDecryptionService();
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

        private string getUnEncryptedDataFromResource()
        {
            using (var unencryptedStream = TestDataUtil.GetContentStreamFromResource("UnencryptedData.txt"))
            using (var bufferStream = new MemoryStream())
            {
                unencryptedStream.CopyTo(bufferStream);
                return Encoding.UTF8.GetString(bufferStream.ToArray());
            }
        }

        private static IDecryptionService CreateDecryptionService()
        {
            return DecryptionService.Create(TestDataUtil.ReadPrivateKey());
        }
    }
}