using System;
using System.IO;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.X509;

namespace KS.Fiks.Crypto
{
    public class BCCryptoService : ICryptoService
    {
        public static class Factory
        {
            public static BCCryptoService Create(X509Certificate certificate)
            {
                if (certificate == null)
                {
                    throw new ArgumentNullException(nameof(certificate));
                }

                return new BCCryptoService(certificate);
            }
        }

        private readonly X509Certificate _certificate;

        public BCCryptoService(X509Certificate certificate)
        {
            _certificate = certificate;
        }

        public Stream Decrypt(Stream encryptedStream)
        {
            throw new NotImplementedException();
        }

        public Stream CreateEncryptionStream()
        {
            var cmsEnvelopedDataStreamGenerator = new CmsEnvelopedDataStreamGenerator();
            cmsEnvelopedDataStreamGenerator.AddKeyTransRecipient(_certificate);
            return new EncodedStream(new MemoryStream(), cmsEnvelopedDataStreamGenerator);
        }

        public Stream Encrypt(Stream unEncryptedStream)
        {
            var encryptionStream = CreateEncryptionStream();
            unEncryptedStream.CopyTo(encryptionStream);
            encryptionStream.Seek(0L, SeekOrigin.Begin);
            return encryptionStream;
        }
    }

    internal class EncodedStream : Stream
    {
        private readonly Stream _backingStream;
        private readonly Stream _cryptoStream;


        public EncodedStream(Stream backingStream, CmsEnvelopedDataStreamGenerator generator)
        {
            _backingStream = backingStream;
            _cryptoStream = generator.Open(_backingStream, CmsEnvelopedGenerator.Aes256Cbc);
        }

        public override void Flush()
        {
            _cryptoStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _backingStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _backingStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _cryptoStream.Write(buffer, offset, count);
        }

        public override bool CanRead => _backingStream.CanRead;
        public override bool CanSeek => _backingStream.CanSeek;
        public override bool CanWrite => _cryptoStream.CanWrite;
        public override long Length => _backingStream.Length;
        public override long Position { get; set; }

        protected override void Dispose(bool disposing)
        {
            _cryptoStream.Dispose();
            _backingStream.Dispose();
        }
    }
}