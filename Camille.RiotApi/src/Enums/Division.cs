using System;

namespace Camille.RiotApi.Enums
{
#if USE_SYSTEXTJSON
  [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
#endif
    public enum Division : byte
    {
        I = 1,
        II = 2,
        III = 3,
        IV = 4,
        [Obsolete] V = 5
    }
}