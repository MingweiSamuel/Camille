using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.ChampionMasteryV4;
using MingweiSamuel.Camille.Enums;

namespace MingweiSamuel.Camille.Test
{
    [TestClass]
    public class ApiChampionMasteryV4Test : ApiTest
    {
        [TestMethod]
        public async Task GetChampionAsync()
        {
            CheckGetChampion(await Api.ChampionMasteryV4.GetChampionMasteryAsync(Region.NA, SummonerIdLugnutsK, ChampionId.Zyra));
        }

        [TestMethod]
        public void GetChampion()
        {
            CheckGetChampion(Api.ChampionMasteryV4.GetChampionMastery(Region.NA, SummonerIdLugnutsK, ChampionId.Zyra));
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
            CheckGetChampions(Api.ChampionMasteryV4.GetAllChampionMasteries(Region.NA, SummonerIdLugnutsK));
        }

        [TestMethod]
        public async Task GetChampionsAsync()
        {
            CheckGetChampions(await Api.ChampionMasteryV4.GetAllChampionMasteriesAsync(Region.NA, SummonerIdLugnutsK));
        }

        public static void CheckGetChampions(ChampionMastery[] champData)
        {
            var topChamps = new HashSet<long>
            {
                ChampionId.Zyra, ChampionId.Soraka, ChampionId.Morgana, ChampionId.Sona, ChampionId.Janna,
                ChampionId.Ekko, ChampionId.Nami, ChampionId.Taric, ChampionId.Poppy, ChampionId.Brand
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
            CheckGetScore(Api.ChampionMasteryV4.GetChampionMasteryScore(Region.EUW, SummonerIdMa5tery));
        }

        [TestMethod]
        public async Task GetScoreAsync()
        {
            CheckGetScore(await Api.ChampionMasteryV4.GetChampionMasteryScoreAsync(Region.EUW, SummonerIdMa5tery));
        }

        public static void CheckGetScore(int score)
        {
            Assert.IsTrue(952 <= score && score < 1000, score.ToString());
        }
    }
}
