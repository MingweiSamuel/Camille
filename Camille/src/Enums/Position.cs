namespace MingweiSamuel.Camille.Enums
{
    public static class Position
    {
        /// <summary>
        /// None role.
        /// For non-positional queue types and for apex tiers
        /// of positional queue types (master, gm, challenger).
        /// </summary>
        public const string NONE = "NONE";

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
