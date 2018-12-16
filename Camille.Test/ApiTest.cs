using MingweiSamuel.Camille;
using System.IO;

namespace Camille.Test
{
    public class ApiTest
    {
        protected const string SummonerIdLugnutsK = "SBM8Ubipo4ge2yj7bhEzL7yvV0C9Oc1XA2l6v5okGMA_nCw";
        protected const string SummonerIdMa5tery  = "IbC4uyFEEW3ZkZw6FZF4bViw3P1EynclAcI6-p-vCpI99Ec";

        protected static readonly RiotApi Api = RiotApi.NewInstance
        (
            new RiotApiConfig.Builder(File.ReadAllText("apikey.txt").Trim())
            {
                MaxConcurrentRequests = 10,
                Retries = 10
            }.Build()
        );
    }
}
