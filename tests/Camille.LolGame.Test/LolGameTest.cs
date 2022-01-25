using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Camille.LolGame.Test
{
    [TestClass]
    [Ignore("Requires LoL to be running.")]
    public class LolGameTest
    {
        private static readonly LolGameApi Api = new LolGameApi();

        [TestMethod]
        public async Task GetActivePlayerAsync()
        {
            var player = await Api.LiveClientData().GetActivePlayerAsync();
            if (null != player)
            {
                Assert.IsNotNull(player.SummonerName);
            }
        }

        [TestMethod]
        public async Task GetActivePlayerAbilitiesAsync()
        {
            var abilities = await Api.LiveClientData().GetActivePlayerAbilitiesAsync();
            if (null != abilities)
            {
                Assert.IsNotNull(abilities.Passive);
                Assert.IsNotNull(abilities.Q);
                Assert.IsNotNull(abilities.W);
                Assert.IsNotNull(abilities.E);
                Assert.IsNotNull(abilities.R);
            }
        }

        [TestMethod]
        public async Task GetActivePlayerNameAsync()
        {
            // Returns empty string if not in a live game.
            var name = await Api.LiveClientData().GetActivePlayerNameAsync();
            Assert.IsNotNull(name);
        }

        [TestMethod]
        public async Task GetActivePlayerRunesAsync()
        {
            var runes = await Api.LiveClientData().GetActivePlayerRunesAsync();
            if (null != runes)
            {
                Assert.IsNotNull(runes.StatRunes);
            }
        }

        [TestMethod]
        public async Task GetAllGameDataAsync()
        {
            var data = await Api.LiveClientData().GetAllGameDataAsync();
            Assert.IsNotNull(data);
            Assert.IsNotNull(data.AllPlayers);
            Assert.IsTrue(0 < data.AllPlayers.Length);

            Assert.IsNotNull(data.ActivePlayer);
            if (null == data.ActivePlayer.Error)
            {
                Assert.IsNotNull(data.ActivePlayer.ChampionStats);
                Assert.IsNotNull(data.ActivePlayer.CurrentGold);
                Assert.IsNotNull(data.ActivePlayer.SummonerName);
            }
            if (null == data.ActivePlayer.SummonerName)
            {
                Assert.IsNull(data.ActivePlayer.ChampionStats);
                Assert.IsNull(data.ActivePlayer.CurrentGold);
                Assert.IsNotNull(data.ActivePlayer.Error);
            }

            Console.WriteLine(data);
        }

        [TestMethod]
        public async Task GetPlayerListAsync()
        {
            var players = await Api.LiveClientData().GetPlayerListAsync();
            Assert.IsNotNull(players);
            Assert.IsTrue(0 < players.Length);
            Assert.IsNotNull(players[0].SummonerName);
        }
    }
}
