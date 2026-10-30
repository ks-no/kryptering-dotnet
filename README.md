# kryptering-dotnet
[![MIT license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/ks-no/kryptering-dotnet/blob/master/LICENSE)
[![Nuget](https://img.shields.io/nuget/vpre/KS.Fiks.Crypto.svg)](https://www.nuget.org/packages/KS.Fiks.Crypto)
[![GitHub issues](https://img.shields.io/github/issues-raw/ks-no/kryptering-dotnet.svg)](//github.com/ks-no/kryptering-dotnet/issues)
[![.NET Library Build and Release](https://github.com/ks-no/kryptering-dotnet/actions/workflows/dotnet-build-release.yml/badge.svg)](https://github.com/ks-no/kryptering-dotnet/actions/workflows/dotnet-build-release.yml)

Common encryption and decryption logic required by various KS FIKS clients on the dotnet platform. All code is released under a MIT license 

All crypto is performed using a portable C# version of the [BouncyCastle library](https://www.bouncycastle.org/csharp/)

## Prerequisites
* This library targets _.Net Core 3.1_ as well as _.Net Standard 2.1_ and _.Net Standard 2.0_.
* A PEM file containing the BASE64 encoded public certificate to be used for encryption
* A PEM file containing the BASE64 encoded private key to be used for decryption

## Example 
### Encryption
```c#
var encryptionService = EncryptionService.Create(/* public certificate */);
using (var encryptedOutStream = /* create out stream */)
using (var dataStream = /* the stream containing the unencrypted data */) 
{
    encryptionService.Encrypt(dataStream, encryptedOutStream);
}
```

### Decryption
```c#
var decryptionService = DecryptionService.Create(/* private key */);
using (var encryptedDataInStream = /* stream containing the encrypted data */)
using (var decryptedOutStream = /* a stream to write the unencrypted data to */)
using (var decryptBufferStream = decryptionService.Decrypt(encryptedDataInStream))
{
    decryptBufferStream.CopyTo(decryptedOutStream);
}
```

### Decryption with multiple private keys

The key-only `DecryptionService.Create` overloads try RSA-OAEP recipients when multiple
private keys are supplied. Messages produced by this library include an RSA-OAEP recipient.
With BouncyCastle 2.7 and later, a failed legacy RSA PKCS#1 v1.5 unwrap returns a random
session key rather than throwing, so it cannot reliably identify the correct private key.

For automatic key selection with legacy-only messages, associate each private key with
its matching certificate. The service matches CMS recipient identifiers (issuer and
serial number, or subject key identifier) before opening the decrypted stream:

```c#
var decryptionService = DecryptionService.Create(new[]
{
    new DecryptionKey(oldCertificate, oldPrivateKey),
    new DecryptionKey(currentCertificate, currentPrivateKey)
});
```

The certificate and private key parameters are BouncyCastle `X509Certificate` and
`AsymmetricKeyParameter` instances. Key/certificate mismatches are rejected when creating
a `DecryptionKey`. No certificate trust or validity check is performed by this association.
If no certificate matches a recipient, decryption throws a `CmsException`.
For legacy-only messages without certificates, select a single private key explicitly;
supplying multiple unassociated keys throws a `CmsException`.
Decrypted content remains streamed; it is not buffered to guess which key succeeded.
