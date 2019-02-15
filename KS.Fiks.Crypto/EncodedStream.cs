using System;
using System.IO;
using Org.BouncyCastle.Cms;

namespace KS.Fiks.Crypto
{
    internal class EncodedStream : Stream
    {
        private readonly Stream backingStream;
        private readonly Stream cryptoStream;

        public EncodedStream(Stream backingStream, CmsEnvelopedDataStreamGenerator generator)
        {
            this.backingStream = backingStream;
            this.cryptoStream = generator.Open(this.backingStream, CmsEnvelopedGenerator.Aes256Cbc);
        }

        public override bool CanRead => this.backingStream.CanRead;

        public override bool CanSeek => this.backingStream.CanSeek;

        public override bool CanWrite => this.cryptoStream.CanWrite;

        public override long Length => this.backingStream.Length;

        public override long Position { get; set; }

        public override void Flush()
        {
            this.cryptoStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.backingStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.backingStream.Seek(offset, origin);
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
            this.cryptoStream.Dispose();
            this.backingStream.Dispose();
        }
    }
}