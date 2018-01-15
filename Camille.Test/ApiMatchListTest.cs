using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.League;
using MingweiSamuel.Camille.Match;

namespace Camille.Test
{
    [TestClass]
    public class ApiMatchListTest : ApiTest
    {
        public const long MillisPerWeek = 7 * TimeSpan.TicksPerDay / TimeSpan.TicksPerMillisecond;
        public const long QueryTime = 1484292409447L;

        [TestMethod]
        public void GetQuery()
        {
            CheckGetQuery(Api.Match.GetMatchlist(Region.NA, 78247, queue: new[] {420},
                beginTime: QueryTime - MillisPerWeek, endTime: QueryTime, champion: new[] {ChampionId.Kalista}));
        }

        [TestMethod]
        public async Task GetQueryAsync()
        {
            CheckGetQuery(await Api.Match.GetMatchlistAsync(Region.NA, 78247, queue: new[] {420},
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
    }
}
