namespace MingweiSamuel.Camille.Enums
{
    /// <summary>
    /// Roles used for positional ranks.
    /// </summary>
    public static class Position
    {
        /// <summary>
        /// None role.
        /// For ranks in non-positional queues.
        /// </summary>
        public const string NONE = "NONE";

        /// <summary>
        /// Apex role.
        /// For ranks in apex tiers (master, gm, challenger)
        /// of positional queue types.
        /// </summary>
        public const string APEX = "APEX";

        /// <summary>Top role.</summary>
        public const string TOP = "TOP";
        /// <summary>Jungle role.</summary>
        public const string JUNGLE = "JUNGLE";
        /// <summary>Mid role.</summary>
        public const string MIDDLE = "MIDDLE";
        /// <summary>Bottom (ADC) role.</summary>
        public const string BOTTOM = "BOTTOM";
        /// <summary>Utility (support) role.</summary>
        public const string UTILITY = "UTILITY";
    }
}
