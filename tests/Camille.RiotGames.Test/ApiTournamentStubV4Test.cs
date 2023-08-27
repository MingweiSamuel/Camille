using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;
using Camille.RiotGames.TournamentStubV4;
using System;
using Camille.RiotGames.Util;

namespace Camille.RiotGames.Test
{
    [TestClass]
    public class ApiTournamentStubV4Test : ApiTest
    {
        [TestMethod]
        [Ignore]
        public async Task CreateTournamentStubAsync()
        {
            var tsV4 = Api.TournamentStubV4();

            var providerId = await tsV4.RegisterProviderDataAsync(
                RegionalRoute.AMERICAS, new ProviderRegistrationParameters()
                {
                    Region = "NA",
                    Url = "https://github.com/MingweiSamuel/Camille",
                });
            Assert.IsNotNull(providerId);

            var tournamentId = await tsV4.RegisterTournamentAsync(
                RegionalRoute.AMERICAS, new TournamentRegistrationParameters()
                {
                    Name = "Camille Tourney :)",
                    ProviderId = providerId,
                });
            Assert.IsNotNull(tournamentId);

            var tourneyCodeParams = new TournamentCodeParameters()
            {
                MapType = "SUMMONERS_RIFT",
                Metadata = "eW91IGZvdW5kIHRoZSBzZWNyZXQgbWVzc2FnZQ==",
                PickType = "TOURNAMENT_DRAFT",
                SpectatorType = "ALL",
                TeamSize = 5,
            };
            var codes = await tsV4.CreateTournamentCodeAsync(
                RegionalRoute.AMERICAS, tourneyCodeParams,
                tournamentId, 1000);
            Assert.IsNotNull(codes);
            Assert.AreEqual(1000, codes.Length);

            Console.WriteLine(string.Join(", ", codes));
        }
    }
}
