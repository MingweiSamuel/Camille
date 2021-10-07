namespace MingweiSamuel.Camille.Enums
{
    public static class TeamId
    {
        /// <summary>Team ID for Summoner's Rift blue side (100).</summary>
        public const int Blue = 100;
        /// <summary>Team ID for Summoner's Rift red side (200).</summary>
        public const int Red = 200;
        /// <summary>"killerTeamId" when Baron Nashor spawns and kills Rift Herald.</summary>
        public const int Other = 300;

        /// <summary>Win string for TeamStats#win. "Win".</summary>
        public const string Win = "Win";
        /// <summary>Loss string for TeamStats#win. "Fail".</summary>
        public const string Lose = "Fail";
    }
}
