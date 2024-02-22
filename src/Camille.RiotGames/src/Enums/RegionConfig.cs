// MobaHinted Copyright (C) 2024 Ethan Henderson <ethan@zbee.codes>
// Licensed under GPLv3 - Refer to the LICENSE file for the complete text

namespace Camille.RiotGames.Enums
{
    public enum RegionConfig
    {
        /// <summary>The Region should be in the URL, as a subdomain</summary>
        InUrlAsSubdomain,

        /// <summary>The Region should be in the URL, as a query parameter</summary>
        InUrlAsRegionQueryParameter,

        /// <summary>The Region should be a header instead of in the URL</summary>
        /// <seealso cref="RiotGamesApiConfig.RegionHeaderKey"/>
        InHeader,
    }
}
