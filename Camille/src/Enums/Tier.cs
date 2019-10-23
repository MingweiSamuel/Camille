namespace MingweiSamuel.Camille.Enums
{
    /// <summary>
    /// Contains tier names (CHALLENGER, MASTER, etc.)
    /// </summary>
    public enum Tier
    {
        CHALLENGER,
        GRANDMASTER,
        MASTER,
        DIAMOND,
        PLATINUM,
        GOLD,
        SILVER,
        BRONZE,
        IRON,

        /// <summary>In most endpoints, tier will not be provided if related summoner is unranked.</summary>
        UNRANKED,
    }
}