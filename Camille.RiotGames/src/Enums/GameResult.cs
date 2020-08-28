namespace Camille.RiotGames.Enums
{
#if USE_NEWTONSOFT
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
#elif USE_SYSTEXTJSON
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
#endif
    public enum GameResult
    {
        /// <summary>Win string for TeamStats#win. "Win".</summary>
        Win,

        /// <summary>Loss string for TeamStats#win. "Fail".</summary>
        Lose
    }
}