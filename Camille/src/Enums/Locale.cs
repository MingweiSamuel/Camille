using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MingweiSamuel.Camille.Enums
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Locale
    {
        public const string en_US = "en_US";
        public const string cs_CZ = "cs_CZ";
        public const string de_DE = "de_DE";
        public const string el_GR = "el_GR";
        public const string en_AU = "en_AU";
        public const string en_GB = "en_GB";
        public const string en_PH = "en_PH";
        public const string en_PL = "en_PL";
        public const string en_SG = "en_SG";
        public const string es_AR = "es_AR";
        public const string es_ES = "es_ES";
        public const string es_MX = "es_MX";
        public const string fr_FR = "fr_FR";
        public const string hu_HU = "hu_HU";
        public const string id_ID = "id_ID";
        public const string it_IT = "it_IT";
        public const string ja_JP = "ja_JP";
        public const string ko_KR = "ko_KR";
        public const string ms_MY = "ms_MY";
        public const string pl_PL = "pl_PL";
        public const string pt_BR = "pt_BR";
        public const string ro_RO = "ro_RO";
        public const string ru_RU = "ru_RU";
        public const string th_TH = "th_TH";
        public const string tr_TR = "tr_TR";
        public const string vn_VN = "vn_VN";
        public const string zh_CN = "zh_CN";
        public const string zh_MY = "zh_MY";
        public const string zh_TW = "zh_TW";

        public static readonly HashSet<string> ValidLocales = new HashSet<string> {
            en_US, cs_CZ, de_DE, el_GR, en_AU, en_GB, en_PH, en_PL, en_SG, es_AR, es_ES, es_MX, fr_FR, hu_HU, id_ID,
            it_IT, ja_JP, ko_KR, ms_MY, pl_PL, pt_BR, ro_RO, ru_RU, th_TH, tr_TR, vn_VN, zh_CN, zh_MY, zh_TW
        };

        public static bool IsSupportedLocale(string str)
        {
            return ValidLocales.Contains(str);
        }
    }
}
