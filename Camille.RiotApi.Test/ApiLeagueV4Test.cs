using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;
using Camille.RiotApi.LeagueV4;

namespace Camille.RiotApi.Test
{
    [TestClass]
    public class ApiLeagueV4Test : ApiTest
    {
//        [TestMethod]
//        [Ignore("Season 9 Positional Ranks Removed")]
//        public void Get()
//        {
//            CheckGet(Api.LeagueV4.GetAllLeaguePositionsForSummoner(Region.NA, SummonerIdC9Sneaky));
//        }
//
//        [TestMethod]
//        [Ignore("Season 9 Positional Ranks Removed")]
//        public async Task GetAsync()
//        {
//            CheckGet(await Api.LeagueV4.GetAllLeaguePositionsForSummonerAsync(Region.NA, SummonerIdC9Sneaky));
//        }
//
//        public static void CheckGet(LeaguePosition[] result)
//        {
//            // C9 Sneaky
//            var rankFound = false;
//            var roleFound = false;
//            foreach (var entry in result)
//            {
//                if (!Queue.RANKED_SOLO_5x5.Equals(entry.QueueType))
//                    continue;
//                rankFound = true;
//                // If he's ranked, Sneaky better be at least Platinum.
//                Assert.IsTrue(
//                    Tier.Platinum == entry.Tier ||
//                    Tier.Diamond == entry.Tier ||
//                    Tier.Master == entry.Tier ||
//                    Tier.Grandmaster == entry.Tier ||
//                    Tier.Challenger == entry.Tier,
//                    entry.Tier);
//                Assert.AreEqual(SummonerIdC9Sneaky, entry.SummonerId);
//                Assert.IsTrue(entry.SummonerName.ToUpperInvariant().Contains("SNEAKY"));
//                roleFound |= entry.Position == Position.BOTTOM || entry.Position == Position.APEX;
//            }
//            Assert.IsTrue(rankFound, "Failed to find queue " + Queue.RANKED_SOLO_5x5 + ", Sneaky unranked.");
//            Assert.IsTrue(roleFound, "Failed to find adc or apex role for queue " +
//                    Queue.RANKED_SOLO_5x5 + ", Sneaky unranked in adc role.");
//        }
//
//        [TestMethod]
//        [Ignore("Season Reset/Season 9 Outdated")] // TODO
//        public void GetApex()
//        {
//            CheckGetApex(Api.LeagueV4.GetChallengerLeague(Region.NA, Queue.RANKED_SOLO_5x5),
//                Api.LeagueV4.GetMasterLeague(Region.NA, Queue.RANKED_SOLO_5x5));
//        }
//
//        [TestMethod]
//        [Ignore("Season Reset/Season 9 Outdated")] // TODO
//        public async Task GetApexAsync()
//        {
//            var challengerTask = Api.LeagueV4.GetChallengerLeagueAsync(Region.NA, Queue.RANKED_SOLO_5x5);
//            var masterTask = Api.LeagueV4.GetMasterLeagueAsync(Region.NA, Queue.RANKED_SOLO_5x5);
//            CheckGetApex(await challengerTask, await masterTask);
//        }
//
//        public static void CheckGetApex(LeagueList challenger, LeagueList master)
//        {
//            Assert.AreEqual(Tier.Challenger, challenger.Tier);
//            Assert.AreEqual("Dr. Mundo's Scouts", challenger.Name); // lol
//            Assert.AreEqual(200, challenger.Entries.Length);
//            var challengerLp = challenger.Entries.Average(e => e.LeaguePoints);
//
//            Assert.AreEqual(Tier.Master, master.Tier);
//            Assert.AreEqual("Renekton's Shadows", master.Name);
//            var masterLp = master.Entries.Average(e => e.LeaguePoints);
//
//            Assert.IsTrue(masterLp < challengerLp,
//                $"Expect average master LP to be less than challenger LP: {masterLp} < {challengerLp}.");
//        }
//
//        [TestMethod]
//        [Ignore("Season 9 Positional Ranks Removed")]
//        public async Task GetQueuesWithPositionRanksAsync()
//        {
//            CheckGetQueuesWithPositionRanks(await Api.LeagueV4.GetQueuesWithPositionRanksAsync(Region.NA));
//        }
//        
//        [TestMethod]
//        [Ignore("Season 9 Positional Ranks Removed")]
//        public void GetQueuesWithPositionRanks()
//        {
//            CheckGetQueuesWithPositionRanks(Api.LeagueV4.GetQueuesWithPositionRanks(Region.NA));
//        }
//
//        public static void CheckGetQueuesWithPositionRanks(string[] queues)
//        {
//            Assert.AreEqual(1, queues.Length);
//            Assert.AreEqual(Queue.RANKED_SOLO_5x5, queues[0]);
//        }
//
//        public const int positionalPage = 0;
//        public const string positionalPosition = Position.JUNGLE;
//        public const string positionalDivision = nameof(Division.I);
//        public const string positionalTier = Tier.Diamond;
//        public const string positionalQueue = Queue.RANKED_SOLO_5x5;
//
//        [TestMethod]
//        [Ignore("Temp Disabled")]
//        public async Task GetPositionalLeagueEntriesAsync()
//        {
//            CheckGetPositionalLeagueEntries(await Api.LeagueV4.GetPositionalLeagueEntriesAsync(Region.NA,
//                positionalPage, positionalPosition, positionalDivision, positionalTier, positionalQueue));
//        }
//        
//        [TestMethod]
//        [Ignore("Temp Disabled")]
//        public void GetPositionalLeagueEntries()
//        {
//            CheckGetPositionalLeagueEntries(Api.LeagueV4.GetPositionalLeagueEntries(Region.NA,
//                positionalPage, positionalPosition, positionalDivision, positionalTier, positionalQueue));
//        }
//
//        public static void CheckGetPositionalLeagueEntries(LeaguePosition[] positions)
//        {
//            //TODO
//        }
    }
}
