using System;

namespace Camille.Enums
{
#if USE_NEWTONSOFT
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
#elif USE_SYSTEXTJSON
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
#endif
    public enum RegionalRoute: byte
    {
        /// <summary>Americas.</summary>
        AMERICAS = 1,
        /// <summary>Asia.</summary>
        ASIA = 2,
        /// <summary>Europe.</summary>
        EUROPE = 3,
        /// <summary>South East Asia. Only usable with the LoR endpoints (just `lorRankedV1` for now).</summary>
        SEA = 4,
    }

#if USE_NEWTONSOFT
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
#elif USE_SYSTEXTJSON
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
#endif
    public enum PlatformRoute: byte
    {
        /// <summary>Brazil.</summary>
        BR1 = 16,

        /// <summary>North-east Europe.</summary>
        EUN1 = 17,

        /// <summary>West Europe.</summary>
        EUW1 = 18,

        /// <summary>Japan.</summary>
        JP1 = 19,

        /// <summary>Korea. Valorant's Korea platform</summary>
        KR = 20,

        /// <summary>North Latin America.</summary>
        LA1 = 21,

        /// <summary>South Latin America.</summary>
        LA2 = 22,

        /// <summary>North America.</summary>
        NA1 = 23,

        /// <summary>Oceania.</summary>
        OC1 = 24,

        /// <summary>Russia.</summary>
        RU = 25,

        /// <summary>Turkey.</summary>
        TR1 = 26,


        /// <summary>Public beta environment. Only functional in certain endpoints. Valorant's PBE platform.</summary>
        PBE1 = 31,
    }

#if USE_NEWTONSOFT
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
#elif USE_SYSTEXTJSON
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
#endif
    public enum ValPlatformRoute: byte
    {
        /// <summary>Valorant's Asian Pacific platform.</summary>
        AP = 64,

        /// <summary>Valorant's Brazil platform.</summary>
        BR = 65,

        /// <summary>Valorant's Europe platform.</summary>
        EU = 66,

        /// <summary>Valorant's Latin America platform.</summary>
        LATAM = 68,

        /// <summary>Valorant's North America platform.</summary>
        NA = 69,
    }

    public static class RouteUtils
    {
        /// <summary>
        /// Converts the PlatformRoute into its corresponding RegionalRoute. Useful for TftMatchV1
        /// endpoints which use regional routes, while the rest of the TFT endpoints use platforms.
        /// </summary>
        /// <param name="value">This, the PlatformRoute to convert.</param>
        /// <returns>AMERICAS, ASIA, or EUROPE. Will not return SEA which is only used for LoR.</returns>
        public static RegionalRoute ToRegional(this PlatformRoute value)
        {
            switch (value)
            {
                case PlatformRoute.BR1:
                case PlatformRoute.LA1:
                case PlatformRoute.LA2:
                case PlatformRoute.NA1:
                case PlatformRoute.OC1:
                case PlatformRoute.PBE1:
                    return RegionalRoute.AMERICAS;

                case PlatformRoute.JP1:
                case PlatformRoute.KR:
                    return RegionalRoute.ASIA;

                case PlatformRoute.EUN1:
                case PlatformRoute.EUW1:
                case PlatformRoute.RU:
                case PlatformRoute.TR1:
                    return RegionalRoute.EUROPE;
            }
            throw new ArgumentException($"Unexpected PlatformRoute value: {value}.");
        }
    }
}
