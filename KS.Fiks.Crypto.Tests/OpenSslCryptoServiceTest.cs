using System;
using System.IO;
using FluentAssertions;
using Moq;
using Xunit;

namespace KS.Fiks.Crypto.Tests
{
    public class OpenSslCryptoServiceTest
    {
        [Fact]
        public void CreateService()
        {
            var openSslCryptoService = OpenSslCryptoService.Factory.Create();
            openSslCryptoService.Should().BeOfType<OpenSslCryptoService>();
        }

        [Fact]
        public void Decrypt()
        {
            var openSslCryptoService = OpenSslCryptoService.Factory.Create();
            var mockStream = new Mock<Stream>();
            openSslCryptoService.Invoking(s => s.Decrypt(mockStream.Object))
                .Should().Throw<NotImplementedException>();
            mockStream.VerifyNoOtherCalls();
        }

        [Fact]
        public void Encrypt()
        {
            var openSslCryptoService = OpenSslCryptoService.Factory.Create();
            var mockStream = new Mock<Stream>();
            openSslCryptoService.Invoking(s => s.Encrypt(mockStream.Object))
                .Should().Throw<NotImplementedException>();
            mockStream.VerifyNoOtherCalls();
        }
    }
}