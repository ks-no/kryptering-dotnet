using System;
using System.IO;
using System.Text;
using KS.Fiks.Crypto.BouncyCastle;
using Moq;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Shouldly;
using Xunit;

namespace KS.Fiks.Crypto.Tests.BouncyCastle
{
    public class DecryptStreamConverterTest
    {
        private const string Plaintext = "Multiple-key decryption must return the exact original content.";
        private static readonly Lazy<DecryptionKey> OtherKey = new Lazy<DecryptionKey>(CreateOtherKey);

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void KeyOnlyDecryptionTriesOaepKeysInEitherOrder(bool validKeyFirst)
        {
            var validKey = TestDataUtil.ReadPrivateKey();
            var otherKey = OtherKey.Value.PrivateKey;
            var keys = validKeyFirst ? new[] { validKey, otherKey } : new[] { otherKey, validKey };
            using var encrypted = Encrypt(true, false, TestDataUtil.ReadPublicCertificate());

            using var decrypted = DecryptionService.Create(keys).Decrypt(encrypted);

            ReadContent(decrypted).ShouldBe(Plaintext);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void CertificateKeysSelectLegacyRecipient(bool validKeyFirst, bool subjectKeyIdentifier)
        {
            var validKey = new DecryptionKey(TestDataUtil.ReadPublicCertificate(), TestDataUtil.ReadPrivateKey());
            var keys = validKeyFirst
                ? new[] { validKey, OtherKey.Value }
                : new[] { OtherKey.Value, validKey };
            using var encrypted = Encrypt(false, subjectKeyIdentifier, validKey.Certificate);

            using var decrypted = DecryptionService.Create(keys).Decrypt(encrypted);

            ReadContent(decrypted).ShouldBe(Plaintext);
        }

        [Fact]
        public void CertificateKeysFindMatchingRecipientBeyondFirst()
        {
            var validKey = new DecryptionKey(TestDataUtil.ReadPublicCertificate(), TestDataUtil.ReadPrivateKey());
            using var encrypted = Encrypt(false, false, OtherKey.Value.Certificate, validKey.Certificate);

            using var decrypted = DecryptionService.Create(new[] { validKey }).Decrypt(encrypted);

            ReadContent(decrypted).ShouldBe(Plaintext);
        }

        [Fact]
        public void KeyOnlyDecryptionTriesOtherOaepRecipients()
        {
            using var encrypted = Encrypt(true, false, OtherKey.Value.Certificate, TestDataUtil.ReadPublicCertificate());
            var service = DecryptionService.Create(new[] { TestDataUtil.ReadPrivateKey(), TestDataUtil.ReadPrivateKey() });

            using var decrypted = service.Decrypt(encrypted);

            ReadContent(decrypted).ShouldBe(Plaintext);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DecryptionRemainsStreamedAndSupportsNonSeekableInput(bool useCertificates)
        {
            var plaintext = new string('x', 3 * 1024 * 1024);
            var certificate = TestDataUtil.ReadPublicCertificate();
            using var encrypted = EncryptContent(plaintext, !useCertificates, false, certificate);
            var input = new Mock<Stream>();
            input.SetupGet(stream => stream.CanRead).Returns(true);
            input.Setup(stream => stream.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns((byte[] buffer, int offset, int count) => encrypted.Read(buffer, offset, count));
            using var inputStream = input.Object;
            var service = useCertificates
                ? DecryptionService.Create(new[]
                {
                    OtherKey.Value, new DecryptionKey(certificate, TestDataUtil.ReadPrivateKey())
                })
                : DecryptionService.Create(new[] { OtherKey.Value.PrivateKey, TestDataUtil.ReadPrivateKey() });

            using var decrypted = service.Decrypt(inputStream);

            encrypted.Position.ShouldBeLessThan(encrypted.Length);
            ReadContent(decrypted).ShouldBe(plaintext);
        }

        [Fact]
        public void SingleKeyStillDecryptsLegacyMessage()
        {
            using var encrypted = Encrypt(false, false, TestDataUtil.ReadPublicCertificate());

            using var decrypted = DecryptStreamConverter.Decrypt(TestDataUtil.ReadPrivateKey(), encrypted);

            ReadContent(decrypted).ShouldBe(Plaintext);
        }

        [Fact]
        public void LegacyMessageRequiresCertificatesForMultipleKeys()
        {
            using var encrypted = Encrypt(false, false, TestDataUtil.ReadPublicCertificate());
            var service = DecryptionService.Create(new[] { OtherKey.Value.PrivateKey, TestDataUtil.ReadPrivateKey() });

            Should.Throw<CmsException>(() => service.Decrypt(encrypted))
                .Message.ShouldContain("certificate-associated");
        }

        [Fact]
        public void NoMatchingCertificateFailsExplicitly()
        {
            using var encrypted = Encrypt(false, false, TestDataUtil.ReadPublicCertificate());

            Should.Throw<CmsException>(() => DecryptionService.Create(new[] { OtherKey.Value }).Decrypt(encrypted))
                .Message.ShouldContain("No CMS recipient matches");
        }

        [Fact]
        public void AllWrongOaepKeysFailBeforeReturningAStream()
        {
            using var encrypted = Encrypt(true, false, TestDataUtil.ReadPublicCertificate());
            var service = DecryptionService.Create(new[] { OtherKey.Value.PrivateKey, OtherKey.Value.PrivateKey });

            Should.Throw<CmsException>(() => service.Decrypt(encrypted)).InnerException.ShouldNotBeNull();
        }

        [Fact]
        public void EmptyKeysAreRejected()
        {
            using var encrypted = new MemoryStream();

            Should.Throw<ArgumentException>(() =>
                DecryptStreamConverter.Decrypt(Array.Empty<AsymmetricKeyParameter>(), encrypted));
            Should.Throw<ArgumentException>(() =>
                DecryptStreamConverter.Decrypt(Array.Empty<DecryptionKey>(), encrypted));
        }

        [Fact]
        public void InvalidKeyEntriesAreRejected()
        {
            using var encrypted = new MemoryStream();

            Should.Throw<ArgumentException>(() =>
                DecryptStreamConverter.Decrypt(new AsymmetricKeyParameter[] { null }, encrypted));
            Should.Throw<ArgumentException>(() =>
                DecryptStreamConverter.Decrypt(new[] { TestDataUtil.ReadPublicCertificate().GetPublicKey() }, encrypted));
            Should.Throw<ArgumentException>(() =>
                DecryptStreamConverter.Decrypt(new DecryptionKey[] { null }, encrypted));
        }

        private static MemoryStream Encrypt(bool oaep, bool subjectKeyIdentifier, params X509Certificate[] certificates)
        {
            return EncryptContent(Plaintext, oaep, subjectKeyIdentifier, certificates);
        }

        private static MemoryStream EncryptContent(string plaintext, bool oaep, bool subjectKeyIdentifier, params X509Certificate[] certificates)
        {
            var generator = new CmsEnvelopedDataStreamGenerator();
            foreach (var certificate in certificates)
            {
                if (oaep)
                {
                    generator.AddKeyTransRecipient("RSA/NONE/OAEPWITHSHA256ANDMGF1PADDING", certificate);
                }
                else if (subjectKeyIdentifier)
                {
                    var identifier = SubjectKeyIdentifier.GetInstance(
                        certificate.GetExtensionValue(X509Extensions.SubjectKeyIdentifier).GetOctets());
                    generator.AddKeyTransRecipient(certificate.GetPublicKey(), identifier.GetKeyIdentifier());
                }
                else
                {
                    generator.AddKeyTransRecipient(certificate);
                }
            }

            using var output = new MemoryStream();
            using (var encrypted = generator.Open(output, CmsEnvelopedGenerator.Aes256Cbc))
            {
                var bytes = Encoding.UTF8.GetBytes(plaintext);
                encrypted.Write(bytes, 0, bytes.Length);
            }

            return new MemoryStream(output.ToArray());
        }

        private static string ReadContent(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        private static DecryptionKey CreateOtherKey()
        {
            var generator = new RsaKeyPairGenerator();
            generator.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
            var pair = generator.GenerateKeyPair();
            var certificateGenerator = new X509V3CertificateGenerator();
            var name = new X509Name("CN=Other decryption test recipient");
            certificateGenerator.SetSerialNumber(BigInteger.Two);
            certificateGenerator.SetIssuerDN(name);
            certificateGenerator.SetSubjectDN(name);
            certificateGenerator.SetNotBefore(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            certificateGenerator.SetNotAfter(new DateTime(2040, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            certificateGenerator.SetPublicKey(pair.Public);
            var certificate = certificateGenerator.Generate(new Asn1SignatureFactory("SHA256WITHRSA", pair.Private));
            return new DecryptionKey(certificate, pair.Private);
        }
    }
}
