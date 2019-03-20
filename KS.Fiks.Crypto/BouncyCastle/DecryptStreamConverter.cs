using System;
using System.IO;
using System.Linq;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;

namespace KS.Fiks.Crypto.BouncyCastle
{
    public static class DecryptStreamConverter
    {
        public static Stream Decrypt(AsymmetricKeyParameter privateKey, Stream encryptedStream)
        {
            if (encryptedStream == null)
            {
                throw new ArgumentNullException(nameof(encryptedStream));
            }

            var pr = privateKey ?? throw new ArgumentNullException(nameof(privateKey));

            var cmsEnvelopedDataParser = new CmsEnvelopedDataParser(new BufferedStream(encryptedStream, 1024 * 1024));
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
            var cs = recipientInformation.GetContentStream(pr);

            return cs.ContentStream;
        }
    }
}