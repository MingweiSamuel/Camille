using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.MatchV3;

namespace Camille.Test
{
    [TestClass]
    public class ApiMatchlistTest : ApiTest
    {
        public const long MillisPerWeek = 7 * TimeSpan.TicksPerDay / TimeSpan.TicksPerMillisecond;
        public const long QueryTime = 1484292409447L;

        [TestMethod]
        public void GetQuery()
        {
            CheckGetQuery(Api.MatchV3.GetMatchlist(Region.NA, 78247, queue: new[] {420},
                beginTime: QueryTime - MillisPerWeek, endTime: QueryTime, champion: new[] {ChampionId.Kalista}));
        }

        [TestMethod]
        public async Task GetQueryAsync()
        {
            CheckGetQuery(await Api.MatchV3.GetMatchlistAsync(Region.NA, 78247, queue: new[] {420},
                beginTime: QueryTime - MillisPerWeek, endTime: QueryTime, champion: new[] {ChampionId.Kalista}));
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

        public static void CheckGetRecent(Matchlist matchlist)
        {
            Assert.IsNotNull(matchlist);
            Assert.IsNotNull(matchlist.Matches);
            //assertEquals(matchlist.totalGames, matchlist.matches.size());

            const long after = 1494737245688L;
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
