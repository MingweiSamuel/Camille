namespace Camille.RiotGames.Enums
{
    public enum RouteConfig
    {
        /// <summary>The Region should be in the URL, inserted where "{0}" is (using string.Format)</summary>
        InUrl,

        /// <summary>The Region should be in the URL query parameters</summary>
        InQueryParam,

        /// <summary>The Region should be a header instead of in the URL</summary>
        /// <seealso cref="RiotGamesApiConfig.RouteKey"/>
        InHeader,
    }
}
