using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;
using System.Linq;
using System.Diagnostics;

namespace Camille.RiotGames.Test
{
    [TestClass]
    public class ValContentV1RankedV1Test : ApiTest
    {
        //public static long[] MatchIds =
        //{
        //    3555874970,
        //    3555881987,
        //    3555758359,
        //    3555745679,
        //    3555760623,
        //};

        [TestMethod]
        public async Task GetContentAsync()
        {
            var content = await Api.ValContentV1().GetContentAsync(ValPlatformRoute.LATAM);
            Assert.IsNotNull(content);
            Console.WriteLine("content: " + string.Join('\n', content._AdditionalProperties.Keys));
            Assert.IsNotNull(content.Characters);
            Assert.IsTrue(0 < content.Characters.Count());

            foreach (var character in content.Characters)
            {
                Assert.IsNotNull(character);
                Assert.IsNotNull(character.Name);
                Assert.IsNotNull(character.AssetName);
                Assert.IsNotNull(character.LocalizedNames);

                Assert.IsNotNull(character.LocalizedNames.ArAE);
                Assert.IsNotNull(character.LocalizedNames.DeDE);
                //Assert.IsNotNull(character.LocalizedNames.EnGB);
                Assert.IsNotNull(character.LocalizedNames.EnUS);
                Assert.IsNotNull(character.LocalizedNames.EsES);
                Assert.IsNotNull(character.LocalizedNames.EsMX);
                Assert.IsNotNull(character.LocalizedNames.FrFR);
                Assert.IsNotNull(character.LocalizedNames.IdID);
                Assert.IsNotNull(character.LocalizedNames.ItIT);
                Assert.IsNotNull(character.LocalizedNames.JaJP);
                Assert.IsNotNull(character.LocalizedNames.KoKR);
                Assert.IsNotNull(character.LocalizedNames.PlPL);
                Assert.IsNotNull(character.LocalizedNames.PtBR);
                Assert.IsNotNull(character.LocalizedNames.RuRU);
                Assert.IsNotNull(character.LocalizedNames.ThTH);
                Assert.IsNotNull(character.LocalizedNames.TrTR);
                Assert.IsNotNull(character.LocalizedNames.ViVN);
                Assert.IsNotNull(character.LocalizedNames.ZhCN);
                Assert.IsNotNull(character.LocalizedNames.ZhTW);
            }
        }

        [TestMethod]
        public async Task GetContentAsyncWithLocaleAndGetLeaderboardAsync()
        {
            var content = await Api.ValContentV1().GetContentAsync(ValPlatformRoute.AP, "ja-JP");
            Assert.IsNotNull(content);
            Assert.IsNotNull(content.Characters);
            Assert.IsTrue(0 < content.Characters.Count());

            foreach (var character in content.Characters)
            {
                Assert.IsNotNull(character);
                Assert.IsNotNull(character.Name);
                Assert.IsNotNull(character.AssetName);
                Assert.IsNull(character.LocalizedNames);
            }

            var act = content.Acts.First(act => act.IsActive);
            var leaderboard = await Api.ValRankedV1().GetLeaderboardAsync(ValPlatformRoute.AP, act.Id);
            Assert.IsTrue(0 < leaderboard.Players.Count());
        }
    }
}
