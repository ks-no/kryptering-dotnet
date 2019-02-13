using System;
using System.IO;
using FluentAssertions;
using Moq;
using Xunit;

namespace KS.Fiks.Crypto.Tests
{
    public class OpenSslCryptoService
    {
        [Fact]
        public void CreateService()
        {
            var openSslCryptoService = Crypto.OpenSslCryptoService.Factory.Create();
            openSslCryptoService.Should().BeOfType<Crypto.OpenSslCryptoService>();
        }

        [Fact]
        public void Encrypt()
        {
            var openSslCryptoService = Crypto.OpenSslCryptoService.Factory.Create();
            var mockStream = new Mock<Stream>();
            openSslCryptoService.Invoking(s => s.Encrypt(mockStream.Object))
                .Should().Throw<NotImplementedException>();
            mockStream.VerifyNoOtherCalls();
        }

        [Fact]
        public void Decrypt()
        {
            var openSslCryptoService = Crypto.OpenSslCryptoService.Factory.Create();
            var mockStream = new Mock<Stream>();
            openSslCryptoService.Invoking(s => s.Decrypt(mockStream.Object))
                .Should().Throw<NotImplementedException>();
            mockStream.VerifyNoOtherCalls();
        }
    }
}