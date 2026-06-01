using System;

namespace NOpenCode
{
    public class NOpenCodeRequestException : NOpenCodeException
    {
        public string Endpoint { get; }
        public int? HttpStatus { get; }
        public string? ResponseBody { get; }

        public NOpenCodeRequestException(string endpoint, string message, int? httpStatus = null, string? responseBody = null, Exception? inner = null)
            : base($"[{httpStatus}] {endpoint}: {message}\n{responseBody ?? ""}", inner)
        {
            Endpoint = endpoint;
            HttpStatus = httpStatus;
            ResponseBody = responseBody;
        }
    }
}
