using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.RiotApi.ChampionMasteryV4;
using Camille.RiotApi.Enums;

namespace Camille.RiotApi.Test
{
    [TestClass]
    public class ApiChampionMasteryV4Test : ApiTest
    {
        [TestMethod]
        public async Task GetChampionAsync()
        {
            CheckGetChampion(await Api.ChampionMasteryV4.GetChampionMasteryAsync(Region.NA1, encryptedSummonerId: SummonerIdLugnutsK, championId: Champion.ZYRA));
        }

        [TestMethod]
        public void GetChampion()
        {
            CheckGetChampion(Api.ChampionMasteryV4.GetChampionMastery(Region.NA1, encryptedSummonerId: SummonerIdLugnutsK, championId: Champion.ZYRA));
        }

        public static void CheckGetChampion(ChampionMastery result)
        {
            Assert.IsNotNull(result);
            Assert.AreEqual(7, result.ChampionLevel, result.ChampionLevel.ToString());
            Assert.IsTrue(result.ChampionPoints >= 389_578, result.ChampionPoints.ToString());
        }

        [TestMethod]
        public void GetChampions()
        {
            CheckGetChampions(Api.ChampionMasteryV4.GetAllChampionMasteries(Region.NA1, SummonerIdLugnutsK));
        }

        [TestMethod]
        public async Task GetChampionsAsync()
        {
            CheckGetChampions(await Api.ChampionMasteryV4.GetAllChampionMasteriesAsync(Region.NA1, SummonerIdLugnutsK));
        }

        public static void CheckGetChampions(ChampionMastery[] champData)
        {
            var topChamps = new HashSet<Champion>
            {
                Champion.ZYRA, Champion.SORAKA, Champion.MORGANA, Champion.SONA, Champion.JANNA,
                Champion.EKKO, Champion.NAMI, Champion.TARIC, Champion.POPPY, Champion.BRAND
            };
            var topChampCount = topChamps.Count;
            for (var i = 0; i < topChampCount; i++)
                Assert.IsTrue(topChamps.Remove(champData[i].ChampionId), $"Unexpected top champ: {champData[i].ChampionId}.");
            Assert.AreEqual(0, topChamps.Count, $"Champions not found: {topChamps}.");
        }

        [TestMethod]
        public void GetScore()
        {
            // http://www.lolking.net/summoner/euw/20401158/0#champ-mastery
            CheckGetScore(Api.ChampionMasteryV4.GetChampionMasteryScore(Region.EUW1, SummonerIdMa5tery));
        }

        [TestMethod]
        public async Task GetScoreAsync()
        {
            CheckGetScore(await Api.ChampionMasteryV4.GetChampionMasteryScoreAsync(Region.EUW1, SummonerIdMa5tery));
        }

        public static void CheckGetScore(int score)
        {
            Assert.IsTrue(952 <= score && score < 1000, score.ToString());
        }
    }
}
