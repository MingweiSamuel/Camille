using Camille.RiotApi.SummonerV4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Camille.RiotApi.Test
{
    [TestClass]
    public class AdditionalPropertiesTest
    {
        [TestMethod]
        public void TestAdditionalProperties()
        {
            var str = @"{
    'id': 'SBM8Ubipo4ge2yj7bhEzL7yvV0C9Oc1XA2l6v5okGMA_nCw',
    'accountId': 'iheZF2uJ50S84Hfq6Ob8GNlJAUmBmac-EsEEWBJjD01q1jQ',
    'puuid': 'bJ_-UdX8v1NqvsUemuklcmd70lNjgDY0UN81L75d84HneX8dHy8iteZC49qEkJVPvGzJZC4R-89dHA',
    'name': 'LugnutsK',
    'oopsExtraField': 'spooky rito',
    'profileIconId': 4540,
    'revisionDate': 1589704662000,
    'summonerLevel': 111
}".Replace('\'', '"');

            var summoner = RiotApi.Deserialize<Summoner>(str);
            Assert.AreEqual(1, summoner._AdditionalProperties.Count);
            Assert.IsTrue(summoner._AdditionalProperties.ContainsKey("oopsExtraField"));
            Assert.AreEqual("spooky rito", summoner._AdditionalProperties["oopsExtraField"].ToString());
        }
    }
}