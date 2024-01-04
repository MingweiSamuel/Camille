using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.RiotGames.ChampionMasteryV4;
using Camille.Enums;

namespace Camille.RiotGames.Test
{
    [TestClass]
    public class ApiChampionMasteryV4Test : ApiTest
    {

        [TestMethod]
        public void GetChampion()
        {
            var summoner = Api.SummonerV4().GetBySummonerName(PlatformRoute.NA1, "LugnutsK");
            CheckGetChampion(Api.ChampionMasteryV4().GetChampionMasteryByPUUID(PlatformRoute.NA1, encryptedPUUID: summoner.Puuid, championId: Champion.ZYRA));
        }

        [TestMethod]
        public async Task GetChampionAsync()
        {
            var summoner = await Api.SummonerV4().GetBySummonerNameAsync(PlatformRoute.NA1, "LugnutsK");
            CheckGetChampion(await Api.ChampionMasteryV4().GetChampionMasteryByPUUIDAsync(PlatformRoute.NA1, encryptedPUUID: summoner.Puuid, championId: Champion.ZYRA));
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
            var summoner = Api.SummonerV4().GetBySummonerName(PlatformRoute.NA1, "LugnutsK");
            CheckGetChampions(Api.ChampionMasteryV4().GetAllChampionMasteriesByPUUID(PlatformRoute.NA1, summoner.Puuid));
        }

        [TestMethod]
        public async Task GetChampionsAsync()
        {
            var summoner = await Api.SummonerV4().GetBySummonerNameAsync(PlatformRoute.NA1, "LugnutsK");
            CheckGetChampions(await Api.ChampionMasteryV4().GetAllChampionMasteriesByPUUIDAsync(PlatformRoute.NA1, summoner.Puuid));
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
        public void GetScoreByPUUID()
        {
            var summoner = Api.SummonerV4().GetBySummonerName(PlatformRoute.EUW1, "Ma5tery");
            CheckGetScore(Api.ChampionMasteryV4().GetChampionMasteryScoreByPUUID(PlatformRoute.EUW1, summoner.Puuid));
        }

        [TestMethod]
        public async Task GetScoreByPUUIDAsync()
        {
            var summoner = await Api.SummonerV4().GetBySummonerNameAsync(PlatformRoute.EUW1, "Ma5tery");
            CheckGetScore(await Api.ChampionMasteryV4().GetChampionMasteryScoreByPUUIDAsync(PlatformRoute.EUW1, summoner.Puuid));
        }

        public static void CheckGetScore(int score)
        {
            Assert.IsTrue(952 <= score && score < 1000, score.ToString());
        }
    }
}
