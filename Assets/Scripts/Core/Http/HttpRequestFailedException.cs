using System;

namespace Core.Http
{
    public class HttpRequestFailedException : Exception
    {
        public long statusCode { get; }
        public string url { get; }

        public HttpRequestFailedException(
            string _message,
            string _url,
            long _statusCode = 0) : base(_message)
        {
            url = _url;
            statusCode = _statusCode;
        }
    }
}