using System;
using System.IO;
using System.Linq;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;

namespace KS.Fiks.Crypto.BouncyCastle
{
    public class DecodedStream : Stream
    {
        private readonly CmsTypedStream cmsTypedStream;
        private readonly Stream dataStream;
        private readonly AsymmetricKeyParameter privateKey;

        public DecodedStream(Stream encryptedStream, AsymmetricKeyParameter privateKey)
        {
            if (encryptedStream == null)
            {
                throw new ArgumentNullException(nameof(encryptedStream));
            }

            this.privateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));

            var cmsEnvelopedDataParser = new CmsEnvelopedDataParser(encryptedStream);
            var encryptionAlgorithm = cmsEnvelopedDataParser.EncryptionAlgOid;
            if (encryptionAlgorithm != CmsEnvelopedGenerator.Aes256Cbc)
            {
                throw new Exception(
                    $"Invalid encryption algorithm detected for CMS message. Expected {CmsEnvelopedGenerator.Aes256Cbc}, got {encryptionAlgorithm}");
            }

            var recipientInformationStore = cmsEnvelopedDataParser.GetRecipientInfos();
            if (recipientInformationStore.Count < 1)
            {
                throw new Exception(
                    $"Number of recipients specified in the CMS is {recipientInformationStore.Count}. Expected < 1");
            }

            var recipients = recipientInformationStore.GetRecipients().Cast<RecipientInformation>();
            var recipientInformation = recipients.AsEnumerable().First(r => true);
            this.cmsTypedStream = recipientInformation.GetContentStream(this.privateKey);
            this.dataStream = this.cmsTypedStream.ContentStream;
        }

        public override bool CanRead => this.dataStream.CanRead;

        public override bool CanSeek => this.dataStream.CanSeek;

        public override bool CanWrite => false;

        public override long Length => this.dataStream.Length;

        public override long Position
        {
            get => this.dataStream.Position;
            set => this.dataStream.Position = value;
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.dataStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.dataStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.dataStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Write is not supported by this stream implementation");
        }

        protected override void Dispose(bool disposing)
        {
            this.dataStream.Dispose();
        }
    }
}