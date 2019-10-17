using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.ChampionV3;
using MingweiSamuel.Camille.Enums;

namespace MingweiSamuel.Camille.Test
{
    [TestClass]
    public class ApiChampionTest : ApiTest
    {
        [TestMethod]
        public void GetChampionInfo()
        {
            CheckGetAll(Api.ChampionV3.GetChampionInfo(Region.NA1));
        }

        [TestMethod]
        public async Task GetChampionInfoAsync()
        {
            CheckGetAll(await Api.ChampionV3.GetChampionInfoAsync(Region.NA1));
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
