using System;
using System.IO;
using Org.BouncyCastle.Cms;

namespace KS.Fiks.Crypto.BouncyCastle
{
    public class EncodedStream : Stream
    {
        private readonly Stream cryptoStream;

        public EncodedStream(Stream backingStream, CmsEnvelopedDataStreamGenerator generator)
        {
            BackingStream = backingStream;

            this.cryptoStream = generator.Open(BackingStream, CmsEnvelopedGenerator.Aes256Cbc);
        }

        public override bool CanRead => BackingStream.CanRead;

        public override bool CanSeek => BackingStream.CanSeek;

        public override bool CanWrite => this.cryptoStream.CanWrite;

        public override long Length => BackingStream.Length;

        public override long Position
        {
            get => BackingStream.Position;
            set => BackingStream.Position = value;
        }

        public Stream BackingStream { get; }

        public override void Flush()
        {
            this.cryptoStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return BackingStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BackingStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.cryptoStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            this.cryptoStream.Close();
            BackingStream.Close();
        }
    }
}