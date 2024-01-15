﻿using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;
using Camille.RiotGames.SpectatorV4;

namespace Camille.RiotGames.Test
{
    [TestClass]
    public class ApiComboFeaturedGamesSummonerCurrentGameV4Test : ApiTest
    {
        public const PlatformRoute REGION = PlatformRoute.EUW1;

        [TestMethod]
        public void Get()
        {
            var featured = Api.SpectatorV4().GetFeaturedGames(REGION);
            CheckFeatured(featured);
            foreach (var gameInfo in featured.GameList)
            {
                var participant = gameInfo.Participants[0];
                var currentGame = Api.SpectatorV4().GetCurrentGameInfoBySummoner(REGION, participant.SummonerId);
                Assert.IsNotNull(currentGame);
                Assert.AreEqual(gameInfo.GameId, currentGame.GameId);
                Assert.IsTrue(currentGame.Participants.Any(cp => participant.SummonerName.Equals(cp.SummonerName)),
                    "Failed to find matching summoner.");
            }
        }

        [TestMethod]
        public async Task GetAsync()
        {
            var featured = await Api.SpectatorV4().GetFeaturedGamesAsync(REGION);
            CheckFeatured(featured);
            var tasks = featured.GameList.Select(async gameInfo =>
            {
                var participant = gameInfo.Participants[0];
                var currentGame = await Api.SpectatorV4().GetCurrentGameInfoBySummonerAsync(REGION, participant.SummonerId);
                Assert.IsNotNull(currentGame);
                Assert.AreEqual(gameInfo.GameId, currentGame.GameId);
                Assert.IsTrue(currentGame.Participants.Any(cp => participant.SummonerName.Equals(cp.SummonerName)),
                    "Failed to find matching summoner.");
            });
            await Task.WhenAll(tasks);
        }

        [TestMethod]
        public void GetParallel()
        {
            var featured = Api.SpectatorV4().GetFeaturedGames(REGION);
            CheckFeatured(featured);
            var result = Parallel.ForEach(featured.GameList, gameInfo =>
            {
                var participant = gameInfo.Participants[0];
                var currentGame = Api.SpectatorV4().GetCurrentGameInfoBySummoner(REGION, participant.SummonerId);
                Assert.IsNotNull(currentGame);
                Assert.AreEqual(gameInfo.GameId, currentGame.GameId);
                Assert.IsTrue(currentGame.Participants.Any(cp => participant.SummonerName.Equals(cp.SummonerName)),
                    "Failed to find matching summoner.");
            });
            Assert.IsTrue(result.IsCompleted);
        }

        public static void CheckFeatured(FeaturedGames featured)
        {
            Assert.IsNotNull(featured);
            Assert.IsNotNull(featured.GameList);
            Assert.AreEqual(5, featured.GameList.Length);
            foreach (var gameInfo in featured.GameList)
            {
                Assert.IsNotNull(gameInfo);
                Assert.IsNotNull(gameInfo.Participants);
                Assert.IsTrue(gameInfo.Participants.Length >= 6);
                foreach (var player in gameInfo.Participants)
                {
                    Assert.IsNotNull(player);
                    Assert.IsNotNull(player.SummonerName);
                    Assert.IsTrue(player.SummonerName.Length > 0);
                    Assert.IsFalse(player.Bot);
                }
                Assert.IsTrue(gameInfo.GameId > 0);
                Assert.IsNotNull(gameInfo.Observers);
                Assert.IsNotNull(gameInfo.Observers.EncryptionKey);
                Assert.IsTrue(Regex.IsMatch(gameInfo.Observers.EncryptionKey, "[a-zA-Z0-9/+]{32}"));
            }
        }
    }
}
