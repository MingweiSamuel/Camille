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
            CheckGetChampion(await Api.ChampionMasteryV4.GetChampionMasteryAsync(Region.NA1, encryptedSummonerId: SummonerIdLugnutsK, championId: (long) Champion.ZYRA));
        }

        [TestMethod]
        public void GetChampion()
        {
            CheckGetChampion(Api.ChampionMasteryV4.GetChampionMastery(Region.NA1, encryptedSummonerId: SummonerIdLugnutsK, championId: (long) Champion.ZYRA));
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
            var topChamps = new HashSet<long>
            {
                (long) Champion.ZYRA, (long) Champion.SORAKA, (long) Champion.MORGANA, (long) Champion.SONA, (long) Champion.JANNA,
                (long) Champion.EKKO, (long) Champion.NAMI, (long) Champion.TARIC, (long) Champion.POPPY, (long) Champion.BRAND
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
