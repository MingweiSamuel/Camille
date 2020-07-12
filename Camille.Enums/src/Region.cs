namespace Camille.Enums
{
#if USE_SYSTEXTJSON
  [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
#endif
    public enum Region
    {
        /// <summary>Brazil.</summary>
        BR1,

        /// <summary>North-east Europe.</summary>
        EUN1,

        /// <summary>West Europe.</summary>
        EUW1,

        /// <summary>North America.</summary>
        NA1,

        /// <summary>Korea. Valorant's Korea platform</summary>
        KR,

        /// <summary>North Latin America.</summary>
        LA1,

        /// <summary>South Latin America.</summary>
        LA2,

        /// <summary>Oceania.</summary>
        OC1,

        /// <summary>Russia.</summary>
        RU,

        /// <summary>Turkey.</summary>
        TR1,

        /// <summary>Japan.</summary>
        JP1,

        /// <summary>Public beta environment. Only functional in certain endpoints. Valorant's PBE platform.</summary>
        PBE1,



        /// <summary>Valorant's Asian Pacific platform.</summary>
        APAC,

        /// <summary>Valorant's Brazil platform.</summary>
        BR,

        /// <summary>Valorant's Europe platform.</summary>
        EU,

        /// <summary>Valorant's Latin America platform.</summary>
        LATAM,

        /// <summary>Valorant's North America platform.</summary>
        NA,



        /// <summary>Garena publisher - South east asia regions. Not functional in endpoints.</summary>
        GARENA,

        /// <summary>Tencent publisher - China. Not functional in endpoints.</summary>
        TENCENT,

        /// <summary>Global.</summary>
        global,



        /// <summary>Regional proxy for Americas.</summary>
        AMERICAS,

        /// <summary>Regional proxy for Europe.</summary>
        EUROPE,

        /// <summary>Regional proxy for Asia.</summary>
        ASIA,



        /// <summary>Tournament Realm 1.</summary>
        TRLH1,

        /// <summary>Tournament Realm 2.</summary>
        TRLH2,

        /// <summary>Tournament Realm 3.</summary>
        TRLH3,

        /// <summary>Tournament Realm 4.</summary>
        TRLH4,
    }
}