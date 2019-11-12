using System;

namespace Camille.Enums
{
#if USE_SYSTEXTJSON
  [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
#endif
    public enum Division : byte
    {
        /// <summary>
        /// "N/A", none available.
        /// </summary>
        NA = 0,

        I = 1,
        II = 2,
        III = 3,
        IV = 4,
        [Obsolete("Removed for 2019.")] V = 5,
    }
}