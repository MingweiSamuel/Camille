using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.SummonerV3;

namespace Camille.Test
{
    [TestClass]
    public class ApiSummonerTest : ApiTest
    {
        private const string SummonerName = "thefizz";

        [TestMethod]
        public void Get()
        {
            var summoner = Api.SummonerV3.GetBySummonerName(Region.RU, SummonerName);
            Console.WriteLine(summoner);
        }

        [TestMethod]
        public async Task GetAsync()
        {
            var summoner = await Api.SummonerV3.GetBySummonerNameAsync(Region.RU, SummonerName);
            Console.WriteLine(summoner);
        }

        public static void CheckGet(Summoner result)
        {
            // TODO
        }
    }
}
