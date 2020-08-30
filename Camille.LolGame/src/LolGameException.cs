using System;
using System.Net.Http;

namespace Camille.LolGame
{
    public class LolGameException : Exception
    {
        public readonly HttpResponseMessage? Response;
        public readonly LolGameErrorMessage? ErrorMessage;
        public readonly string? ResponseContent;

        public LolGameException(HttpResponseMessage? response, LolGameErrorMessage? errorMessage, string? responseContent)
        {
            Response = response;
            ErrorMessage = errorMessage;
            ResponseContent = responseContent;
        }
        public LolGameException(Exception cause, HttpResponseMessage? response, LolGameErrorMessage? errorMessage, string? responseContent) : base("", cause)
        {
            Response = response;
            ErrorMessage = errorMessage;
            ResponseContent = responseContent;
        }
        public LolGameException(string message, HttpResponseMessage? response, LolGameErrorMessage? errorMessage, string? responseContent) : base(message)
        {
            Response = response;
            ErrorMessage = errorMessage;
            ResponseContent = responseContent;
        }
        public LolGameException(string message, Exception cause, HttpResponseMessage? response, LolGameErrorMessage? errorMessage, string? responseContent) : base(message, cause)
        {
            Response = response;
            ErrorMessage = errorMessage;
            ResponseContent = responseContent;
        }
    }
}
