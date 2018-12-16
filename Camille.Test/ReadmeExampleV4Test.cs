using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Enums;

namespace Camille.Test
{
    [TestClass]
    public class ReadmeExampleTest : ApiTest
    {
        [TestMethod]
        [Ignore("Static Data")]
        public void SummonerChampionMasteryTest()
        {
            // Use existing instance for test.
            var riotApi = Api;

            // Get summoners by name synchronously. (using async is faster).
            var summoners = new[]
            {
                riotApi.SummonerV4.GetBySummonerName(Region.NA, "c9 sneaky"),
                riotApi.SummonerV4.GetBySummonerName(Region.NA, "double LIFT")
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
                    var champ = mastery.ChampionId.ToString();
                    // print i, champ id, champ mastery points, and champ level
                    Console.WriteLine("{0,3}) {1,-16} {2,7} ({3})", i + 1, champ,
                        mastery.ChampionPoints, mastery.ChampionLevel);
                }
                Console.WriteLine();
            }
        }
    }
}
