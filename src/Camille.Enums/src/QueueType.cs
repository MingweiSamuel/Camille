using System;

namespace Camille.Enums
{
#if USE_NEWTONSOFT
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
#elif USE_SYSTEXTJSON
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
#endif
    public enum QueueType
    {
        /// <summary>Returned by LCU only.</summary>
        [Obsolete("Returned by LCU only.")]
        NONE = 0,
        /// <summary>Ranked solo queue, 5v5 on Summoner's Rift.</summary>
        RANKED_SOLO_5x5 = 420,
        /// <summary>Ranked flex pick, 5v5 on Summoner's Rift.</summary>
        RANKED_FLEX_SR = 440,
        /// <summary>Ranked flex pick, 3v3 on Twisted Treeline.</summary>
        RANKED_FLEX_TT = 470,
        /// <summary>Ranked Teamfight Tactics.</summary>
        RANKED_TFT = 1100,
        /// <summary>Ranked Teamfight Tactics, Hyper Roll game mode.</summary>
        RANKED_TFT_TURBO = 1130,
        /// <summary>Ranked Teamfight Tactics, Double Up game mode.</summary>
        [Obsolete("Deprecated in patch 12.11 in favor of queueId 1160")]
        RANKED_TFT_PAIRS = 1150,
        /// <summary>Ranked Teamfight Tactics, Double Up game mode.</summary>
        RANKED_TFT_DOUBLE_UP = 1160,

        CHERRY = 1700
    }
}