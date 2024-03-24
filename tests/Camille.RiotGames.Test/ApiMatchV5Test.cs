using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;
using System.Linq;


namespace Camille.RiotGames.Test
{
    [TestClass]
    public class ApiMatchV5Test : ApiTest
    {
        public static string[] MatchIds =
        {
            // New ARENA 2v2v2v2 game mode
            "EUW1_6511808246", // https://github.com/MingweiSamuel/Camille/issues/99
            // Added 2023-08-27
            "EUW1_6569580003",
            "EUW1_6569417645",
            "EUW1_6568707352",
            "EUW1_6568635198",
            "EUW1_6568537080",
            // https://github.com/MingweiSamuel/Camille/issues/99
            "EUW1_6511808246",
            // 2024-03-23 from spectator featured games
            "EUW1_6869252626",
        };

        [TestMethod]
        public async Task GetMatchAsync()
        {
            var tasks = MatchIds
                .Select(id => Api.MatchV5().GetMatchAsync(RegionalRoute.EUROPE, id))
                .ToList();
            var matches = await Task.WhenAll(tasks);

            foreach (var (match, id) in matches.Zip(MatchIds, (match, id) => (match, id)))
            {
                Assert.IsNotNull(match, $"Match for id {id} is null.");
                Assert.AreEqual(id, match.Metadata.MatchId);
                Assert.IsTrue(id.EndsWith("" + match.Info.GameId), $"Requested match id {id}'s gameId is {match.Info.GameId} unexpectedly.");

                Assert.AreEqual(2, match.Info.Teams.Length);

                Assert.IsNotNull(match.Info.Participants);

                foreach (var participant in match.Info.Participants)
                {
                    Assert.IsNotNull(participant, $"Null participant in match {id}.");
                    Assert.IsTrue(0 < participant.ChampionId, $"Invalid champion {participant.ChampionId} for participant in match {id}.");
                    Assert.IsNotNull(participant.ParticipantId, $"Null `ParticipantId` in match {id}.");
                    Assert.IsNotNull(participant.SummonerId, $"Null `SummonerId` in match {id}.");
                    Assert.IsNotNull(participant.RiotIdName ?? participant.RiotIdGameName, $"Null `RiotIdName ?? RiotIdGameName` in match {id}.");
                    Assert.IsNotNull(participant.RiotIdTagline, $"Null `RiotIdTagline` in match {id}.");
                }
            }
        }
    }
}
