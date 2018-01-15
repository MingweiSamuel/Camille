using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Champion;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.League;
using MingweiSamuel.Camille.Match;

namespace Camille.Test
{
    [TestClass]
    public class ApiLeagueTest : ApiTest
    {
        [TestMethod]
        public void Get()
        {
            CheckGet(Api.League.GetAllLeaguePositionsForSummoner(Region.NA, 51405));
        }

        [TestMethod]
        public async Task GetAsync()
        {
            CheckGet(await Api.League.GetAllLeaguePositionsForSummonerAsync(Region.NA, 51405));
        }

        public static void CheckGet(LeaguePosition[] result)
        {
            // C9 Sneaky
            foreach (var entry in result)
            {
                if (!Queue.RANKED_SOLO_5x5.Equals(entry.QueueType))
                    continue;
                // If he's ranked, Sneaky better be at least Diamond
                Assert.IsTrue(
                    Tier.Diamond == entry.Tier ||
                    Tier.Master == entry.Tier ||
                    Tier.Challenger == entry.Tier,
                    entry.Tier);
                Assert.AreEqual("51405", entry.PlayerOrTeamId);
                Assert.IsTrue(entry.PlayerOrTeamName.ToUpperInvariant().Contains("SNEAKY"));
            }
            Console.WriteLine("Failed to find queue " + Queue.RANKED_SOLO_5x5 + ", Sneaky unranked.");
        }
    }
}
