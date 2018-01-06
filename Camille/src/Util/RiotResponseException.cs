using System;
using System.Net;

namespace MingweiSamuel.Camille.Util
{
    public class RiotResponseException : Exception
    {
        private readonly HttpWebResponse _response;

        public RiotResponseException(HttpWebResponse response)
        {
            _response = response;
        }
        public RiotResponseException(Exception cause, HttpWebResponse response) : base("", cause)
        {
            _response = response;
        }
        public RiotResponseException(string message, HttpWebResponse response) : base(message)
        {
            _response = response;
        }
        public RiotResponseException(string message, Exception cause, HttpWebResponse response) : base(message, cause)
        {
            _response = response;
        }

        public HttpWebResponse GetResponse()
        {
            return _response;
        }
    }
}
