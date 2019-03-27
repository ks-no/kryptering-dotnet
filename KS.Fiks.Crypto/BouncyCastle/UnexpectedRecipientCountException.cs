using System;

namespace KS.Fiks.Crypto.BouncyCastle
{
    /// <summary>
    /// Exception thrown when the number of recipients in a CMS message was less than expected
    /// </summary>
    public class UnexpectedRecipientCountException : Exception
    {
        public UnexpectedRecipientCountException(int actualRecipientCount) : this(
            $"Number of recipients specified in the CMS is {actualRecipientCount}. Expected > 0")
        {
        }

        private UnexpectedRecipientCountException()
        {
        }

        private UnexpectedRecipientCountException(string message) : base(message)
        {
        }

        private UnexpectedRecipientCountException(string message, Exception innerException = null) :
            base(message, innerException)
        {
        }
    }
}