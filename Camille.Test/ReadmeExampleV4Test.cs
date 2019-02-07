using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Enums;

namespace Camille.Test
{
    [TestClass]
    public class ReadmeExampleTest : ApiTest
    {
        [TestMethod]
        public void SummonerChampionMasteryTest()
        {
            // Use existing instance for test.
            var riotApi = Api;

            // Get summoners by name synchronously. (using async is faster).
            var summoners = new[]
            {
                riotApi.SummonerV4.GetBySummonerName(Region.NA, "jAnna kendrick"),
                riotApi.SummonerV4.GetBySummonerName(Region.NA, "lug nuts k")
            };

            foreach (var summoner in summoners)
            {
                Console.WriteLine($"{summoner.Name}'s Top 10 Champs:");

                var masteries =
                    riotApi.ChampionMasteryV4.GetAllChampionMasteries(Region.NA, summoner.Id);

                for (var i = 0; i < 10; i++)
                {
                    var mastery = masteries[i];
                    // Get champion for this mastery.
                    var champ = (Champion) mastery.ChampionId;
                    // print i, champ id, champ mastery points, and champ level
                    Console.WriteLine("{0,3}) {1,-16} {2,10:N0} ({3})", i + 1, champ.Name(),
                        mastery.ChampionPoints, mastery.ChampionLevel);
                }
                Console.WriteLine();
            }
        }
    }
}
