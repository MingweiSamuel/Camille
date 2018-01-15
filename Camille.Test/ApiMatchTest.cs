using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.Match;
using Match = MingweiSamuel.Camille.Match.Match;

namespace Camille.Test
{
    [TestClass]
    public class ApiMatchTest : ApiTest
    {
        [TestMethod]
        public void Get()
        {
            CheckGet(Api.Match.GetMatch(Region.NA, 2357244372L));
        }
        
        [TestMethod]
        public async Task GetAsync()
        {
            CheckGet(await Api.Match.GetMatchAsync(Region.NA, 2357244372L));
        }
        public static void CheckGet(Match match)
        {
            // http://matchhistory.na.leagueoflegends.com/en/#match-details/NA1/2398184332/51405?tab=overview
            Assert.AreEqual(2357244372L, match.GameId);
            Assert.AreEqual(11, match.MapId);
            Assert.AreEqual(1636, match.GameDuration);
            var c9Sneaky = false;
            foreach (var identity in match.ParticipantIdentities)
                if (identity.Player.SummonerId == 51405)
                    c9Sneaky = true;
            Assert.IsTrue(c9Sneaky, "C9 Sneaky not found");
            Assert.AreEqual(2, match.Teams.Length);
            int[] bans = { 0, ChampionId.Syndra, ChampionId.Rengar, ChampionId.LeBlanc, ChampionId.Ivern, ChampionId.Ryze, ChampionId.Amumu};
            foreach (var team in match.Teams)
            {
                if (team.TeamId == TeamId.Blue)
                {
                    Assert.AreEqual("Win", team.Win, team.Win);
                    Assert.IsTrue(team.FirstBlood, team.FirstBlood.ToString());
                }
                else
                {
                    Assert.AreEqual("Fail", team.Win, team.Win);
                    Assert.IsFalse(team.FirstBlood, team.FirstBlood.ToString());
                }
                Assert.AreEqual(3, team.Bans.Length);
                foreach (var ban in team.Bans)
                    Assert.AreEqual(bans[ban.PickTurn], ban.ChampionId);
            }
        }

        [TestMethod]
        public void GetTimeline()
        {
            CheckGetTimeline(Api.Match.GetMatchTimeline(Region.NA, 2357244372));
        }

        [TestMethod]
        public async Task GetTimelineAsync()
        {
            CheckGetTimeline(await Api.Match.GetMatchTimelineAsync(Region.NA, 2357244372));
        }

        public static void CheckGetTimeline(MatchTimeline timeline)
        {
            Assert.AreEqual(60_000, timeline.FrameInterval);
            Assert.IsNotNull(timeline.Frames);
            Assert.AreEqual(29, timeline.Frames.Length);
            long time = 0;
            foreach (var frame in timeline.Frames)
            {
                // frames don't exactly match, they are a bit delayed.
                Assert.IsTrue(frame.Timestamp >= time, "actual " + frame.Timestamp + " < expected " + time);
                // but not delayed by more than a second.
                Assert.IsTrue(frame.Timestamp <= time + 1000, "actual " + frame.Timestamp + " > expected " + (time + 1000));
                time = frame.Timestamp + 60_000;

                var participantGold = new Dictionary<int, int>();
                foreach (var participantFrameEntry in frame.ParticipantFrames)
                {
                    Assert.IsNotNull(participantFrameEntry);
                    Assert.AreEqual((long) participantFrameEntry.Key, participantFrameEntry.Value.ParticipantId);

                    // Check gold increasing.
                    participantGold.TryGetValue(participantFrameEntry.Key, out var prevGold);
                    var currGold = participantFrameEntry.Value.TotalGold;
                    participantGold[participantFrameEntry.Key] = currGold;

                    Assert.AreNotEqual(0, currGold);
                    if (prevGold != 0)
                        Assert.IsTrue(currGold > prevGold, currGold + " > " + prevGold);
                }
            }
        }
    }
}
