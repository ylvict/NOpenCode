using System;

namespace NOpenCode
{
    public class NOpenCodeException : Exception
    {
        public NOpenCodeException() { }
        public NOpenCodeException(string message) : base(message) { }
        public NOpenCodeException(string message, Exception? inner) : base(message, inner ?? null!) { }
    }
}
