namespace Camille.RiotApi.Enums
{
    /// <summary>
    /// Contains tier names (CHALLENGER, MASTER, etc.)
    /// </summary>
#if USE_SYSTEXTJSON
  [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
#endif
    public enum Tier : byte
    {
        CHALLENGER  =  20,
        GRANDMASTER =  40,
        MASTER      =  60,
        DIAMOND     =  80,
        PLATINUM    = 100,
        GOLD        = 120,
        SILVER      = 140,
        BRONZE      = 160,
        IRON        = 180,

        /// <summary>Used by some LCU endpoints.</summary>
        NONE = byte.MaxValue,
    }
}