#if USE_NEWTONSOFT
using JsonPropertyAttribute = Newtonsoft.Json.JsonPropertyAttribute;
#elif USE_SYSTEXTJSON
using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;
#else
#error One of USE_NEWTONSOFT or USE_SYSTEXTJSON must be set.
#endif

namespace Camille.LolGame
{
    public class LolGameErrorMessage
    {
#nullable disable
        public LolGameErrorMessage() { }
#nullable restore

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("httpStatus")]
        public int HttpStatus { get; set; }

        [JsonProperty("implementationDetails")]
        public object ImplementationDetails { get; set; }

        /// <summary>
        /// If this is set to "Spectator mode doesn't currently support this feature" then you are calling a
        /// active-player endpoint while spectating.
        /// </summary>
        [JsonProperty("message")]
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
