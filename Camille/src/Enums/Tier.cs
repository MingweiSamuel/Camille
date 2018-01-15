namespace MingweiSamuel.Camille.Enums
{
    /// <summary>
    /// Contains tier names (CHALLENGER, MASTER, etc.)
    /// </summary>
    public static class Tier
    {
        public const string Challenger = "CHALLENGER";
        public const string Master = "MASTER";
        public const string Diamond = "DIAMOND";
        public const string Platinum = "PLATINUM";
        public const string Gold = "GOLD";
        public const string Silver = "SILVER";
        public const string Bronze = "BRONZE";
        /// <summary>In most endpoints, tier will not be provided if related summoner is unranked.</summary>
        public const string Unranked = "UNRANKED";
    }
}
