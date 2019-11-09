using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;
using Camille.RiotApi.SummonerV4;

namespace Camille.RiotApi.Test
{
    [TestClass]
    public class ApiSummonerTest : ApiTest
    {

        [TestMethod]
        public void Get()
        {
            CheckGet(Api.SummonerV4.GetBySummonerName(Region.NA1, "50550639DEL1"));
        }

        [TestMethod]
        public async Task GetAsync()
        {
            CheckGet(await Api.SummonerV4.GetBySummonerNameAsync(Region.NA1, "50550639DEL1"));
        }

        [TestMethod]
        public void GetUnicode()
        {
            CheckGet(Api.SummonerV4.GetBySummonerName(Region.EUW1, "相当猥琐"));
        }

        [TestMethod]
        public async Task GetUnicodeAsync()
        {
            CheckGet(await Api.SummonerV4.GetBySummonerNameAsync(Region.EUW1, "相当猥琐"));
        }

        public static void CheckGet(Summoner summoner)
        {
            Console.WriteLine(summoner);
            Assert.IsNotNull(summoner);
        }
    }
}
