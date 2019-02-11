using System;
using System.Runtime.Serialization;

namespace AzureFunctionsUpdates.Exceptions
{
    public class RepoException : Exception
    {
        public RepoException()
        {
        }

        public RepoException(string message) : base(message)
        {
        }

        public RepoException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RepoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
