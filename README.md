# kryptering-dotnet
[![Apache V2 License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/ks-no/kryptering-dotnet/blob/master/LICENSE)

Common encryption and decryption logic required by various KS FIKS clients on the dotnet platform. All code is released under a MIT license 

All crypto is performed using a portable version of the [BouncyCastle library](https://www.bouncycastle.org/csharp/) 

## Prerequisites
* This library targets _.Net Core 2.2_
* A PEM file containing the BASE64 encoded public certificate to be used for encryption
* A PEM file containing the BASE64 encoded private key to be used for decryption

## Example 
### Encryption
```c#
var cryptoService = BCCryptoService.Create(/* public certificate */, /* private key */);
using (var encryptedOutStream = /* create out stream */)
using (var dataStream = /* the stream containing the unencrypted data */) 
{
    cryptoService.Encrypt(dataStream, encryptedOutStream);
}
```

### Decryption
```c#
var cryptoService = BCCryptoService.Create(/* public certificate */, /* private key */);
using (var encryptedDataInStream = /* stream containing the encrypted data */)
using (var decryptedOutStream = /* a stream to write the unencrypted data to */)
using (var decryptBufferStream = cryptoService.Decrypt(encryptedDataInStream))
{
    decryptBufferStream.CopyTo(decryptedOutStream);
}
```