# kryptering-dotnet
[![MIT license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/ks-no/kryptering-dotnet/blob/master/LICENSE)
[![Nuget](https://img.shields.io/nuget/vpre/KS.Fiks.Crypto.svg)](https://www.nuget.org/packages/KS.Fiks.Crypto)
[![GitHub issues](https://img.shields.io/github/issues-raw/ks-no/kryptering-dotnet.svg)](//github.com/ks-no/kryptering-dotnet/issues)

Common encryption and decryption logic required by various KS FIKS clients on the dotnet platform. All code is released under a MIT license 

All crypto is performed using a portable C# version of the [BouncyCastle library](https://www.bouncycastle.org/csharp/)

## Prerequisites
* This library targets _.Net Core 3.1_ as well as _.Net Standard 2.1_
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
