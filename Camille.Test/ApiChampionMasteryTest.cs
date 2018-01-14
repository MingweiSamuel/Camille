using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.ChampionMastery;
using MingweiSamuel.Camille.Enums;

namespace Camille.Test
{
    [TestClass]
    public class ApiChampionMasteryTest : ApiTest
    {
        [TestMethod]
        public async Task GetChampionAsync()
        {
            CheckGetChampion(await Api.ChampionMastery.GetChampionMasteryAsync(Region.NA, 69009277, ChampionId.Zyra));
        }

        [TestMethod]
        public void GetChampion()
        {
            CheckGetChampion(Api.ChampionMastery.GetChampionMastery(Region.NA, 69009277, ChampionId.Zyra));
        }

        public static void CheckGetChampion(ChampionMastery result)
        {
            Assert.IsNotNull(result);
            Assert.AreEqual(7, result.ChampionLevel, result.ChampionLevel.ToString());
            Assert.IsTrue(result.ChampionPoints >= 389_578, result.ChampionPoints.ToString());
        }
    }
}
