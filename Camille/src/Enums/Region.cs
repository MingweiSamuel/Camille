using System;
#pragma warning disable 618

namespace MingweiSamuel.Camille.Enums
{
    public struct Region
    {
        #region normal regions
        /// <summary>Brazil.</summary>
        public static readonly Region BR = new Region("BR", "BR1");
        /// <summary>North-east Europe.</summary>
        public static readonly Region EUNE = new Region("EUNE", "EUN1");
        /// <summary>West Europe.</summary>
        public static readonly Region EUW = new Region("EUW", "EUW1");
        /// <summary>North America.</summary>
        public static readonly Region NA = new Region("NA", "NA1");
        /// <summary>Korea.</summary>
        public static readonly Region KR = new Region("KR", "KR");
        /// <summary>North Latin America.</summary>
        public static readonly Region LAN = new Region("LAN", "LA1");
        /// <summary>South Latin America.</summary>
        public static readonly Region LAS = new Region("LAS", "LA2");
        /// <summary>Oceania.</summary>
        public static readonly Region OCE = new Region("OCE", "OC1");
        /// <summary>Russia.</summary>
        public static readonly Region RU = new Region("RU", "RU");
        /// <summary>Turkey.</summary>
        public static readonly Region TR = new Region("TR", "TR1");
        /// <summary>Japan.</summary>
        public static readonly Region JP = new Region("JP", "JP1");
        #endregion

        #region other regions
        /// <summary>Public beta environment. Only functional in certain endpoints.</summary>
        public static readonly Region PBE = new Region("PBE", "PBE1");
        /// <summary>Garena publisher - South east asia regions. Not functional in endpoints.</summary>
        public static readonly Region Garena = new Region("GARENA", null);
        /// <summary>Tencent publisher - China. Not functional in endpoints.</summary>
        public static readonly Region Tencent = new Region("TENCENT", null);
        /// <summary>Global.</summary>
        public static readonly Region Global = new Region("GLOBAL", "global");
        #endregion

        #region regional proxies
        /// <summary>Regional proxy for Americas.</summary>
        public static readonly Region Americas = new Region("AMERICAS", "AMERICAS");
        /// <summary>Regional proxy for Europe.</summary>
        public static readonly Region Europe = new Region("EUROPE", "EUROPE");
        /// <summary>Regional proxy for Asia.</summary>
        public static readonly Region Asia = new Region("ASIA", "ASIA");
        #endregion

        #region tournament realms
        /// <summary>Tournament Realm 1.</summary>
        public static readonly Region TRLH1 = new Region("TRLH", "TRLH1");
        /// <summary>Tournament Realm 2.</summary>
        public static readonly Region TRLH2 = new Region("TRLH", "TRLH2");
        /// <summary>Tournament Realm 3.</summary>
        public static readonly Region TRLH3 = new Region("TRLH", "TRLH3");
        /// <summary>Tournament Realm 4.</summary>
        public static readonly Region TRLH4 = new Region("TRLH", "TRLH4");
        #endregion

        /// <summary>Region key in all capital letters.</summary>
        public readonly string Key;
        /// <summary>Platform ID in capital letters and digits.</summary>
        public readonly string Platform;

        [Obsolete("Use Region.* static regions instead.")]
        public Region(string key, string platform)
        {
            Key = key;
            Platform = platform;
        }
    }
}
