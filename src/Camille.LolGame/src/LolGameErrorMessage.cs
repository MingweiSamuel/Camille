using System.Text.Json.Serialization;

namespace Camille.LolGame
{
    public class LolGameErrorMessage
    {
#nullable disable
        public LolGameErrorMessage() { }
#nullable restore

        [JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; }

        [JsonPropertyName("httpStatus")]
        public int HttpStatus { get; set; }

        [JsonPropertyName("implementationDetails")]
        public object ImplementationDetails { get; set; }

        /// <summary>
        /// If this is set to "Spectator mode doesn't currently support this feature" then you are calling a
        /// active-player endpoint while spectating.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        public override string ToString()
        {
            return "LolGameErrorMessage("
                + "ErrorCode: " + ErrorCode + ", "
                + "HttpStatus: " + HttpStatus + ", "
                + "ImplementationDetails: " + ImplementationDetails + ", "
                + "Message: " + Message + ")";
        }
    }
}
