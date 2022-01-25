using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;
using Camille.RiotGames.LorRankedV1;
using System.Linq;

namespace Camille.RiotGames.Test
{
    [TestClass]
    public class ApiLorRankedV1Test : ApiTest
    {
        [TestMethod]
        [Ignore("LOR key needed")]
        public async Task GetLeaderboardsAsync()
        {
            CheckGet(await Api.LorRankedV1().GetLeaderboardsAsync(RegionalRoute.AMERICAS));
        }

        public static void CheckGet(Leaderboard leaderboard)
        {
            Assert.IsNotNull(leaderboard.Players);
            Assert.IsTrue(0 < leaderboard.Players.Length);
            // Make sure that LP was parsed correctly since it looks like a float.
            Assert.IsTrue(leaderboard.Players.Any(player => 0 < player.Lp));
        }
    }
}
