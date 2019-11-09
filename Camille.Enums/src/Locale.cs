﻿namespace Camille.Enums
{
#if USE_SYSTEXTJSON
  [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
#endif
    public enum Locale
    {
        en_US,
        cs_CZ,
        de_DE,
        el_GR,
        en_AU,
        en_GB,
        en_PH,
        en_PL,
        en_SG,
        es_AR,
        es_ES,
        es_MX,
        fr_FR,
        hu_HU,
        id_ID,
        it_IT,
        ja_JP,
        ko_KR,
        ms_MY,
        pl_PL,
        pt_BR,
        ro_RO,
        ru_RU,
        th_TH,
        tr_TR,
        vn_VN,
        zh_CN,
        zh_MY,
        zh_TW
    }
}