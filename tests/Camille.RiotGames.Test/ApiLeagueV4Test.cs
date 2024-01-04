using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;

namespace Camille.RiotGames.Test
{
    [TestClass]
    public class ApiLeagueV4Test : ApiTest
    {
        [TestMethod]
        public async Task GetChallengerLeagueAsync()
        {
            var data = await Api.LeagueV4().GetChallengerLeagueAsync(PlatformRoute.KR, QueueType.RANKED_SOLO_5x5);
            foreach (var entry in data.Entries)
            {
                Assert.IsTrue(entry.Wins > 0);
            }
        }

        [TestMethod]
        public async Task GetLeagueEntriesAsync()
        {
            var summoner = await Api.SummonerV4().GetBySummonerNameAsync(PlatformRoute.RU, "d3atomiz3d");
            var entries = await Api.LeagueV4().GetLeagueEntriesForSummonerAsync(PlatformRoute.RU, summoner.Id);
            var _ = entries;
        }
    }
}
