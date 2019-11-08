namespace Camille.RiotApi.Enums
{
#if USE_SYSTEXTJSON
  [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
#endif
    public enum Team
    {
        /// <summary>Team ID for Summoner's Rift blue side (100).</summary>
        Blue = 100,

        /// <summary>Team ID for Summoner's Rift red side (200).</summary>
        Red = 200,
    }
}