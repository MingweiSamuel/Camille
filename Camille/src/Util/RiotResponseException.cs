using System;
using System.Net.Http;

namespace MingweiSamuel.Camille.Util
{
    public class RiotResponseException : Exception
    {
        private readonly HttpResponseMessage _response;

        public RiotResponseException(HttpResponseMessage response)
        {
            _response = response;
        }
        public RiotResponseException(Exception cause, HttpResponseMessage response) : base("", cause)
        {
            _response = response;
        }
        public RiotResponseException(string message, HttpResponseMessage response) : base(message)
        {
            _response = response;
        }
        public RiotResponseException(string message, Exception cause, HttpResponseMessage response) : base(message, cause)
        {
            _response = response;
        }

        public HttpResponseMessage GetResponse()
        {
            return _response;
        }
    }
}
