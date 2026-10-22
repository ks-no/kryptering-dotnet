using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;

namespace KS.Fiks.Crypto.BouncyCastle
{
    public static class DecryptStreamConverter
    {
        public static Stream Decrypt(AsymmetricKeyParameter privateKey, Stream encryptedStream)
        {
            if (privateKey == null)
            {
                throw new ArgumentNullException(nameof(privateKey));
            }

            return Decrypt(new[] { privateKey }, encryptedStream);
        }

        public static Stream Decrypt(IReadOnlyCollection<AsymmetricKeyParameter> privateKeys, Stream encryptedStream)
        {
            if (privateKeys == null)
            {
                throw new ArgumentNullException(nameof(privateKeys));
            }

            if (privateKeys.Count == 0 || privateKeys.Any(key => key == null || !key.IsPrivate))
            {
                throw new ArgumentException("At least one private key is required, and all keys must be private.", nameof(privateKeys));
            }

            var recipients = GetRecipientInformation(encryptedStream);
            if (privateKeys.Count == 1)
            {
                return recipients[0].GetContentStream(privateKeys.First()).ContentStream;
            }

            // PKCS#1 v1.5 implicit rejection returns a random session key for an incorrect key.
            // Only OAEP unwrap failures can be used to try another key without reading the content.
            var oaepRecipients = recipients
                .Where(recipient => recipient.KeyEncryptionAlgOid == PkcsObjectIdentifiers.IdRsaesOaep.Id)
                .ToList();
            if (oaepRecipients.Count == 0)
            {
                throw new CmsException("Multiple private keys require an RSA-OAEP recipient or certificate-associated DecryptionKey entries.");
            }

            Exception lastException = null;
            foreach (var recipient in oaepRecipients)
            {
                foreach (var privateKey in privateKeys)
                {
                    try
                    {
                        return recipient.GetContentStream(privateKey).ContentStream;
                    }
                    catch (CmsException exception) when (
                        exception.InnerException is InvalidCipherTextException ||
                        exception.InnerException is DataLengthException)
                    {
                        lastException = exception;
                    }
                    catch (InvalidOperationException exception)
                    {
                        // RSA's fault check also rejects malformed private keys before content is opened.
                        lastException = exception;
                    }
                }
            }

            throw new CmsException("None of the private keys could decrypt an RSA-OAEP recipient.", lastException);
        }

        public static Stream Decrypt(IReadOnlyCollection<DecryptionKey> keys, Stream encryptedStream)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            if (keys.Count == 0 || keys.Any(key => key == null))
            {
                throw new ArgumentException("At least one certificate-associated key is required, and entries must not be null.", nameof(keys));
            }

            foreach (var recipient in GetRecipientInformation(encryptedStream))
            {
                var key = keys.FirstOrDefault(candidate => recipient.RecipientID.Match(candidate.Certificate));
                if (key != null)
                {
                    return recipient.GetContentStream(key.PrivateKey).ContentStream;
                }
            }

            throw new CmsException("No CMS recipient matches the supplied certificates.");
        }

        private static List<RecipientInformation> GetRecipientInformation(Stream encryptedStream)
        {
            if (encryptedStream == null)
            {
                throw new ArgumentNullException(nameof(encryptedStream));
            }

            var cmsEnvelopedDataParser = new CmsEnvelopedDataParser(new BufferedStream(encryptedStream, 1024 * 1024));
            var encryptionAlgorithm = cmsEnvelopedDataParser.EncryptionAlgOid;
            if (encryptionAlgorithm != CmsEnvelopedGenerator.Aes256Cbc)
            {
                throw new UnexpectedEncryptionAlgorithmException(CmsEnvelopedGenerator.Aes256Cbc, encryptionAlgorithm);
            }

            var recipientInformationStore = cmsEnvelopedDataParser.GetRecipientInfos();
            if (recipientInformationStore.Count < 1)
            {
                throw new UnexpectedRecipientCountException(recipientInformationStore.Count);
            }

            return recipientInformationStore.GetRecipients().Cast<RecipientInformation>()
                .OrderByDescending(recipient => recipient.KeyEncryptionAlgOid == PkcsObjectIdentifiers.IdRsaesOaep.Id)
                .ToList();
        }
    }
}