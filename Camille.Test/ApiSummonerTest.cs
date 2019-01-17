using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.SummonerV4;

namespace Camille.Test
{
    [TestClass]
    public class ApiSummonerTest : ApiTest
    {

        [TestMethod]
        public void Get()
        {
            CheckGet(Api.SummonerV4.GetBySummonerName(Region.NA, "50550639DEL1"));
        }

        [TestMethod]
        public async Task GetAsync()
        {
            CheckGet(await Api.SummonerV4.GetBySummonerNameAsync(Region.NA, "50550639DEL1"));
        }

        [TestMethod]
        public void GetUnicode()
        {
            CheckGet(Api.SummonerV4.GetBySummonerName(Region.EUW, "相当猥琐"));
        }

        [TestMethod]
        public async Task GetUnicodeAsync()
        {
            CheckGet(await Api.SummonerV4.GetBySummonerNameAsync(Region.EUW, "相当猥琐"));
        }

        public static void CheckGet(Summoner summoner)
        {
            Console.WriteLine(summoner);
            Assert.IsNotNull(summoner);
        }
    }
}
