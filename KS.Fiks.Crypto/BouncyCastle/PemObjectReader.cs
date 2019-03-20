using System.IO;
using Org.BouncyCastle.Utilities.IO.Pem;

namespace KS.Fiks.Crypto.BouncyCastle
{
    public static class PemObjectReader
    {
        public static PemObject ReadPem(Stream inputStream)
        {
            using (var pemStream = new StreamReader(inputStream))
            {
                return ReadPemObject(new PemReader(pemStream));
            }
        }

        public static PemObject ReadPem(string pemString)
        {
            using (var pemPublicStringReader = new StringReader(pemString))
            {
                return ReadPemObject(new PemReader(pemPublicStringReader));
            }
        }

        private static PemObject ReadPemObject(PemReader pemReader)
        {
            return pemReader.ReadPemObject();
        }
    }
}