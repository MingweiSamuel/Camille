using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.TftLeagueV1;

namespace MingweiSamuel.Camille.Test
{
    [TestClass]
    public class ApiTftLeagueV1Test : ApiTest
    {

        [TestMethod]
        public async Task GetChallengerAsync()
        {
            CheckGetChallenger(await Api.TftLeagueV1.GetChallengerLeagueAsync(Region.EUW));
        }

        public static void CheckGetChallenger(LeagueList leagueList)
        {
            Console.WriteLine(leagueList);
            if (null == leagueList)
            {
                Console.WriteLine("No one in TFT Challenger!");
                return;
            }

            Assert.IsNotNull(leagueList);
            Assert.IsNotNull(leagueList.Entries);
        }
    }
}
