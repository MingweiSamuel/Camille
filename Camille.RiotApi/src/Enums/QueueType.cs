namespace Camille.RiotApi.Enums
{
#if USE_SYSTEXTJSON
  [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
#endif
    public enum QueueType
    {
        RANKED_SOLO_5x5 = 420,
        RANKED_TFT = 1100,
        RANKED_FLEX_SR = 440,
        RANKED_FLEX_TT = 470
    }
}