using System;
using System.Net;
using System.Runtime.Serialization;
using Onyx.ShiftScheduler.Core.Interfaces;

namespace Onyx.ShiftScheduler.Core.Exceptions
{
    [Serializable]
    public class ApplicationBadRequestException : ApplicationException, IException
    {
        public ApplicationBadRequestException()
        {
            StatusCode = HttpStatusCode.BadRequest;
        }

        public ApplicationBadRequestException(string message) : base(message)
        {
            Error = message;
        }

        public ApplicationBadRequestException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        public ApplicationBadRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
            Error = message;
        }

        public HttpStatusCode StatusCode { get; set; }

        public string Error { get; set; }
    }
}