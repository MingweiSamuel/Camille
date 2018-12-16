using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Enums;

namespace Camille.Test
{
    [TestClass]
    public class ReadmeExampleV3Test : ApiTest
    {
        [TestMethod]
        [Ignore("Static Data")]
        public void SummonerChampionMasteryTest()
        {
            // Use existing instance for test.
            var riotApi = Api;

//            // Get champion static data (for champion names).
//            // Note the LolStaticData endpoints have very low rate limits (10/hr).
//            var champs = riotApi.LolStaticData.GetChampionList(Region.NA, dataById: true).Data;

            // Get summoners by name synchronously. (using async is faster).
            var summoners = new[]
            {
                riotApi.SummonerV3.GetBySummonerName(Region.NA, "c9 sneaky"),
                riotApi.SummonerV3.GetBySummonerName(Region.NA, "double LIFT")
            };

            foreach (var summoner in summoners)
            {
                Console.WriteLine($"{summoner.Name}'s Top 10 Champs:");

                var masteries =
                    riotApi.ChampionMasteryV3.GetAllChampionMasteries(Region.NA, summoner.Id);

                for (var i = 0; i < 10; i++)
                {
                    var mastery = masteries[i];
                    // Get champion for this mastery.
                    var champ = mastery.ChampionId.ToString();
                    // print i, champ id, champ mastery points, and champ level
                    Console.WriteLine("{0,3}) {1,-16} {2,7} ({3})", i + 1, champ, //.Name,
                        mastery.ChampionPoints, mastery.ChampionLevel);
                }
                Console.WriteLine();
            }
        }
    }
}
