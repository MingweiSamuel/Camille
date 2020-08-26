using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;
using Camille.RiotApi.SummonerV4;

namespace Camille.RiotApi.Test
{
    [TestClass]
    public class ApiSummonerV4Test : ApiTest
    {

        [TestMethod]
        public void Get()
        {
            CheckGet(Api.SummonerV4().GetBySummonerName(PlatformRoute.NA1, "50550639DEL1"));
        }

        [TestMethod]
        public async Task GetAsync()
        {
            CheckGet(await Api.SummonerV4().GetBySummonerNameAsync(PlatformRoute.NA1, "50550639DEL1"));
        }

        [TestMethod]
        public void GetUnicode()
        {
            CheckGet(Api.SummonerV4().GetBySummonerName(PlatformRoute.EUW1, "相当猥琐"));
        }

        [TestMethod]
        public async Task GetUnicodeAsync()
        {
            CheckGet(await Api.SummonerV4().GetBySummonerNameAsync(PlatformRoute.EUW1, "相当猥琐"));
        }

        public static void CheckGet(Summoner summoner)
        {
            Console.WriteLine(summoner);
            Assert.IsNotNull(summoner);
        }

        [TestMethod]
        public void GetNonexistentSummoner()
        {
            Assert.IsNull(Api.SummonerV4().GetBySummonerName(PlatformRoute.JP1, "this summoner does not exist"));
        }

        [TestMethod]
        public async Task GetNonexistentSummonerAsync()
        {
            Assert.IsNull(await Api.SummonerV4().GetBySummonerNameAsync(PlatformRoute.JP1, "this summoner does not exist"));
        }
    }
}
