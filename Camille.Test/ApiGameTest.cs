using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Champion;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.Match;

namespace Camille.Test
{
    [TestClass]
    public class ApiGameTest : ApiTest
    {
        [TestMethod]
        public void GetRecent()
        {
            CheckGetRecent(Api.Match.GetRecentMatchlist(Region.NA, 78247));
        }

        [TestMethod]
        public async Task GetRecentAsync()
        {
            CheckGetRecent(await Api.Match.GetRecentMatchlistAsync(Region.NA, 78247));
        }

        public static void CheckGetRecent(Matchlist result)
        {
            Assert.AreEqual(20, result.Matches.Length);
            foreach (var game in result.Matches)
            {
                Assert.IsTrue(game.Timestamp >= 1515311457534, game.Timestamp.ToString());
            }
        }
    }
}
