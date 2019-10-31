using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.TftSummonerV1;

namespace MingweiSamuel.Camille.Test
{
    [TestClass]
    [Ignore("API key doesn't include TFT APIs.")]
    public class ApiTftSummonerV1Test : ApiTest
    {

        [TestMethod]
        public async Task GetAsync()
        {
            CheckGet(await Api.TftSummonerV1.GetBySummonerNameAsync(Region.NA, "50550639DEL1"));
        }

        [TestMethod]
        public async Task GetUnicodeAsync()
        {
            CheckGet(await Api.TftSummonerV1.GetBySummonerNameAsync(Region.EUW, "相当猥琐"));
        }

        public static void CheckGet(Summoner summoner)
        {
            Console.WriteLine(summoner);
            Assert.IsNotNull(summoner);
        }
    }
}
