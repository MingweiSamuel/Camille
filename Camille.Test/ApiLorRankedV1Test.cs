using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.LorRankedV1;


namespace MingweiSamuel.Camille.Test
{
    [TestClass]
    public class ApiLorRankedV1Test : ApiTest
    {
        [TestMethod]
        [Ignore("LOR key needed")]
        public async Task GetLeaderboardsAsync()
        {
            CheckGet(await Api.LorRankedV1.GetLeaderboardsAsync(Region.Americas));
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
