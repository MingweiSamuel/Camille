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
    }
}
