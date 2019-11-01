namespace MingweiSamuel.Camille.Enums
{
    /// <summary>
    /// Contains tier names (CHALLENGER, MASTER, etc.)
    /// </summary>
#if USE_SYSTEXTJSON
  [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
#endif
    public enum Tier
    {
        CHALLENGER,
        GRANDMASTER,
        MASTER,
        DIAMOND,
        PLATINUM,
        GOLD,
        SILVER,
        BRONZE,
        IRON,

        /// <summary>In most endpoints, tier will not be provided if related summoner is unranked.</summary>
        UNRANKED,
    }
}