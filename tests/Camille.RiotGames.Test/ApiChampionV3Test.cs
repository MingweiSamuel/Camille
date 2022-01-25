using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.RiotGames.ChampionV3;
using Camille.Enums;

namespace Camille.RiotGames.Test
{
    [TestClass]
    public class ApiChampionV3Test : ApiTest
    {
        [TestMethod]
        public void GetChampionInfo()
        {
            CheckGetAll(Api.ChampionV3().GetChampionInfo(PlatformRoute.NA1));
        }

        [TestMethod]
        public async Task GetChampionInfoAsync()
        {
            CheckGetAll(await Api.ChampionV3().GetChampionInfoAsync(PlatformRoute.NA1));
        }

        public static void CheckGetAll(ChampionInfo result)
        {
            // We're up to 14 free champions (2017/08).
            Assert.IsTrue(10 <= result.FreeChampionIds.Length);

            Assert.IsTrue(0 <= result.FreeChampionIdsForNewPlayers.Length);
            Assert.IsTrue(0 != result.MaxNewPlayerLevel);
        }
    }
}
