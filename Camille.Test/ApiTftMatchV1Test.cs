using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.TftMatchV1;

namespace MingweiSamuel.Camille.Test
{
    [TestClass]
    [Ignore("API key doesn't include TFT APIs.")]
    public class ApiTftMatchV1Test : ApiTest
    {
        [TestMethod]
        public async Task GetAsync()
        {
            CheckGet(await Api.TftMatchV1.GetMatchAsync(Region.Americas, "PBE1_4328907912"));
        }

        public static void CheckGet(Match match)
        {
            Console.WriteLine(match);
            Assert.IsNotNull(match);
        }
    }
}
