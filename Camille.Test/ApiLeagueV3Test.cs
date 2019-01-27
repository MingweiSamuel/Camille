using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.LeagueV3;

namespace Camille.Test
{
    [TestClass]
    public class ApiLeagueV3Test : ApiTest
    {
        [TestMethod]
        public void Get()
        {
            CheckGet(Api.LeagueV3.GetAllLeaguePositionsForSummoner(Region.NA, 51405));
        }

        [TestMethod]
        public async Task GetAsync()
        {
            CheckGet(await Api.LeagueV3.GetAllLeaguePositionsForSummonerAsync(Region.NA, 51405));
        }

        public static void CheckGet(LeaguePosition[] result)
        {
            // C9 Sneaky
            foreach (var entry in result)
            {
                if (!Queue.RANKED_SOLO_5x5.Equals(entry.QueueType))
                    continue;
                // If he's ranked, Sneaky better be at least Platinum.
                Assert.IsTrue(
                    Tier.Platinum == entry.Tier ||
                    Tier.Diamond == entry.Tier ||
                    Tier.Master == entry.Tier ||
                    Tier.Grandmaster == entry.Tier ||
                    Tier.Challenger == entry.Tier,
                    entry.Tier);
                Assert.AreEqual("51405", entry.PlayerOrTeamId);
                Assert.IsTrue(entry.PlayerOrTeamName.ToUpperInvariant().Contains("SNEAKY"));
                return;
            }
            Console.WriteLine("Failed to find queue " + Queue.RANKED_SOLO_5x5 + ", Sneaky unranked.");
        }

        [TestMethod]
        [Ignore("Season Reset/Season 9 Outdated")]
        public void GetTop()
        {
            CheckGetTop(Api.LeagueV3.GetChallengerLeague(Region.NA, Queue.RANKED_SOLO_5x5),
                Api.LeagueV3.GetMasterLeague(Region.NA, Queue.RANKED_SOLO_5x5));
        }

        [TestMethod]
        [Ignore("Season Reset/Season 9 Outdated")]
        public async Task GetTopAsync()
        {
            var challengerTask = Api.LeagueV3.GetChallengerLeagueAsync(Region.NA, Queue.RANKED_SOLO_5x5);
            var masterTask = Api.LeagueV3.GetMasterLeagueAsync(Region.NA, Queue.RANKED_SOLO_5x5);
            CheckGetTop(await challengerTask, await masterTask);
        }

        public static void CheckGetTop(LeagueList challenger, LeagueList master)
        {
            Assert.AreEqual(Tier.Challenger, challenger.Tier);
            Assert.AreEqual("Dr. Mundo's Scouts", challenger.Name); // lol
            Assert.AreEqual(200, challenger.Entries.Length);
            var challengerLp = challenger.Entries.Average(e => e.LeaguePoints);

            Assert.AreEqual(Tier.Master, master.Tier);
            Assert.AreEqual("Renekton's Shadows", master.Name);
            var masterLp = master.Entries.Average(e => e.LeaguePoints);

            Assert.IsTrue(masterLp < challengerLp,
                $"Expect average master LP to be less than challenger LP: {masterLp} < {challengerLp}.");
        }
    }
}
