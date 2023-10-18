using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;
using Camille.RiotGames.TournamentStubV5;
using System;

namespace Camille.RiotGames.Test
{
    [TestClass]
    public class ApiTournamentStubV5Test : ApiTest
    {
        [TestMethod]
        public async Task CreateTournamentStubAsync()
        {
            var tsV4 = Api.TournamentStubV5();

            var providerId = await tsV4.RegisterProviderDataAsync(
                RegionalRoute.AMERICAS, new ProviderRegistrationParametersV5()
                {
                    Region = "NA",
                    Url = "https://github.com/MingweiSamuel/Camille",
                });
            Assert.IsNotNull(providerId);

            var tournamentId = await tsV4.RegisterTournamentAsync(
                RegionalRoute.AMERICAS, new TournamentRegistrationParametersV5()
                {
                    Name = "Camille Tourney :)",
                    ProviderId = providerId,
                });
            Assert.IsNotNull(tournamentId);

            var tourneyCodeParams = new TournamentCodeParametersV5()
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
