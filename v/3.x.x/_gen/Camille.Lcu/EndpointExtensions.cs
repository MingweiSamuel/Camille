﻿
// This file is automatically generated.
// Do not directly edit.
// Generated on 2021-07-03T02:12:03.760Z

// Version 1.0.0

namespace Camille.Lcu
{
    public static class LcuExtensions
    {
        public static LolChampSelectEndpoints LolChampSelect(this ILcuApi @base)
        {
            return new LolChampSelectEndpoints(@base);
        }

        public static LolLoginEndpoints LolLogin(this ILcuApi @base)
        {
            return new LolLoginEndpoints(@base);
        }

        public static LolRankedEndpoints LolRanked(this ILcuApi @base)
        {
            return new LolRankedEndpoints(@base);
        }

        public static LolSummonerEndpoints LolSummoner(this ILcuApi @base)
        {
            return new LolSummonerEndpoints(@base);
        }

        public static RiotClientEndpoints RiotClient(this ILcuApi @base)
        {
            return new RiotClientEndpoints(@base);
        }

    }
}

