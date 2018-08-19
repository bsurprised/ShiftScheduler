using System;
using System.Net;
using System.Runtime.Serialization;
using Onyx.ShiftScheduler.Core.Interfaces;

namespace Onyx.ShiftScheduler.Core.Exceptions
{
    [Serializable]
    public class ApplicationErrorException : ApplicationException, IException
    {
        public ApplicationErrorException()
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }

        public ApplicationErrorException(string message) : base(message)
        {
            Error = message;
        }

        public ApplicationErrorException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        public ApplicationErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
            Error = message;
        }

        public HttpStatusCode StatusCode { get; set; }

        public string Error { get; set; }
    }
}