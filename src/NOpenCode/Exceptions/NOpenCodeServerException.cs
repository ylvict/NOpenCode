using System;

namespace NOpenCode
{
    public class NOpenCodeServerException : NOpenCodeException
    {
        public int? StatusCode { get; }

        public NOpenCodeServerException(string message, int? statusCode = null)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public NOpenCodeServerException(string message, Exception inner, int? statusCode = null)
            : base(message, inner)
        {
            StatusCode = statusCode;
        }
    }
}
