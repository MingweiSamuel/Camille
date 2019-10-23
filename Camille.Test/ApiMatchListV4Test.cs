using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.MatchV4;

namespace MingweiSamuel.Camille.Test
{
    [TestClass]
    [Ignore("Requires updating")] // TODO
    public class ApiMatchlistTestV4 : ApiTest
    {
        public const long MillisPerWeek = 7 * TimeSpan.TicksPerDay / TimeSpan.TicksPerMillisecond;
        public const long QueryTime = 1484292409447L;

        [TestMethod]
        public void GetQuery()
        {
            CheckGetQuery(Api.MatchV4.GetMatchlist(Region.NA1, AccountIdC9Sneaky, queue: new[] {RankedQueues.RANKED_SOLO_5x5},
                beginTime: QueryTime - MillisPerWeek, endTime: QueryTime, champion: new[] {Champion.KALISTA}));
        }

        [TestMethod]
        public async Task GetQueryAsync()
        {
            CheckGetQuery(await Api.MatchV4.GetMatchlistAsync(Region.NA1, AccountIdC9Sneaky, queue: new[] { RankedQueues.RANKED_SOLO_5x5 },
                beginTime: QueryTime - MillisPerWeek, endTime: QueryTime, champion: new[] {Champion.KALISTA}));
        }

        public static void CheckGetQuery(Matchlist matchlist)
        {
            Assert.IsNotNull(matchlist);
            Assert.AreEqual(1, matchlist.TotalGames);
            Assert.AreEqual(0, matchlist.StartIndex);
            Assert.AreEqual(1, matchlist.EndIndex);
            Assert.IsNotNull(matchlist.Matches);

            long[] expected = { 2398184332L };
            Assert.AreEqual(expected.Length, matchlist.Matches.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.IsNotNull(matchlist.Matches[i]);
                Assert.AreEqual(expected[i], matchlist.Matches[i].GameId);
            }
        }

        [TestMethod]
        public void GetQueryRecent()
        {
            CheckGetQueryRecent(Api.MatchV4.GetMatchlist(Region.NA1, AccountIdC9Sneaky, queue: new[] { RankedQueues.RANKED_SOLO_5x5 }));
        }

        [TestMethod]
        public async Task GetQueryRecentAsync()
        {
            CheckGetQueryRecent(await Api.MatchV4.GetMatchlistAsync(Region.NA1, AccountIdC9Sneaky, queue: new[] { RankedQueues.RANKED_SOLO_5x5 }));
        }

        public static void CheckGetQueryRecent(Matchlist matchlist)
        {
            Assert.IsNotNull(matchlist);
            Assert.IsNotNull(matchlist.Matches);
            //assertEquals(matchlist.totalGames, matchlist.matches.size());

            const long after = 1494737245688L;
            //long max = 0;
            var timestamp = long.MaxValue;
            foreach (var match in matchlist.Matches)
            {
                Assert.IsNotNull(match);
                Assert.IsTrue(match.Timestamp >= after);
                // check descending
                Assert.IsTrue(match.Timestamp < timestamp);
                timestamp = match.Timestamp;
            }
        }
    }
}
