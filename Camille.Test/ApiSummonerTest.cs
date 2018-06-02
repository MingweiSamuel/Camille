using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Champion;
using MingweiSamuel.Camille.Enums;

namespace Camille.Test
{
    [TestClass]
    public class ApiSummonerTest : ApiTest
    {
        private const string SummonerName = "thefizz";

        [TestMethod]
        public void Get()
        {
            var summoner = Api.Summoner.GetBySummonerName(Region.RU, SummonerName);
            Console.WriteLine(summoner);
        }

        [TestMethod]
        public async Task GetAsync()
        {
            var summoner = await Api.Summoner.GetBySummonerNameAsync(Region.RU, SummonerName);
            Console.WriteLine(summoner);
        }

        public static void CheckGet(ChampionList result)
        {

        }
    }
}
