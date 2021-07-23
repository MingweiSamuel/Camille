using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;
using System.Linq;


namespace Camille.RiotGames.Test
{
    [TestClass]
    [Ignore("No access with CI API key")]
    public class ApiMatchV5Test : ApiTest
    {
        public static string[] MatchIds =
        {
            "EUW1_5378349967",
        };

        [TestMethod]
        public async Task GetMatchAsync()
        {
            var tasks = MatchIds
                .Select(id => Api.MatchV5().GetMatchAsync(RegionalRoute.EUROPE, id))
                .ToList();
            var matches = await Task.WhenAll(tasks);

            foreach (var ( match, id ) in matches.Zip(MatchIds, (match, id) => ( match, id )))
            {
                Assert.IsNotNull(match);
                Assert.AreEqual(id, match.Metadata.MatchId);
                Assert.IsTrue(id.EndsWith("" + match.Info.GameId), $"Requested match id {id}'s gameId is {match.Info.GameId} unexpectedly.");

                Assert.AreEqual(2, match.Info.Teams.Length);

                Assert.IsNotNull(match.Info.Participants);

                foreach (var participant in match.Info.Participants)
                {
                    Assert.IsNotNull(participant);
                    Assert.IsTrue(0 < participant.ChampionId);
                    Assert.IsNotNull(participant.ParticipantId);
                    Assert.IsNotNull(participant.SummonerId);
                    Assert.IsNotNull(participant.RiotIdName);
                    Assert.IsNotNull(participant.RiotIdTagline);
                }
            }
        }
    }
}
