namespace Camille.RiotApi.Enums
{
#if USE_SYSTEXTJSON
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