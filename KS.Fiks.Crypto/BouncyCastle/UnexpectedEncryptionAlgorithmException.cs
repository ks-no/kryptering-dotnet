using System;

namespace KS.Fiks.Crypto.BouncyCastle
{
    /// <summary>
    /// Thrown when the provided encryption algorithm is not the one that was expected
    /// </summary>
    public class UnexpectedEncryptionAlgorithmException : Exception
    {
        public UnexpectedEncryptionAlgorithmException(string expectedAlgorithm, string providedAlgorithm) :
            base(
                $"Invalid encryption algorithm detected for CMS message. Expected {expectedAlgorithm}, got {providedAlgorithm}")
        {
        }

        private UnexpectedEncryptionAlgorithmException()
        {
        }

        private UnexpectedEncryptionAlgorithmException(
            string message) :
            base(message)
        {
        }

        private UnexpectedEncryptionAlgorithmException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}