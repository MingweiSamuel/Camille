using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;
using System.Linq;

namespace Camille.RiotGames.Test
{
    [TestClass]
    public class ApiMatchV4Test : ApiTest
    {
        public static long[] MatchIds =
        {
            3555874970,
            3555881987,
            3555758359,
            3555745679,
            3555760623,
        };

        [TestMethod]
        public async Task GetMatchAsync()
        {
            var tasks = MatchIds
                .Select(id => Api.MatchV4().GetMatchAsync(PlatformRoute.NA1, id))
                .ToList();
            var matches = await Task.WhenAll(tasks);

            foreach (var ( match, id ) in matches.Zip(MatchIds, (match, id) => ( match, id )))
            {
                Assert.IsNotNull(match);
                Assert.AreEqual(id, match.GameId);

                Assert.IsNotNull(match.Participants);
                Assert.IsNotNull(match.ParticipantIdentities);
                Assert.AreEqual(match.Participants.Count(), match.ParticipantIdentities.Count());

                foreach (var participant in match.Participants)
                {
                    Assert.IsNotNull(participant);
                    Assert.IsTrue(0 < participant.ChampionId);
                    Assert.IsNotNull(participant.Stats);

                    Assert.IsNotNull(participant.Stats._AdditionalProperties);
                    if (0 < participant.Stats._AdditionalProperties.Count())
                        Console.WriteLine(string.Join('\n', participant.Stats._AdditionalProperties.Keys));
                }
            }
        }
    }
}
